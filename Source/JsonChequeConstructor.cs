using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using static FR_Operator.FiscalPrinter;

namespace FR_Operator
{
    public class JsonChequeConstructor
    {
        public static FnReadedDocument ConvertToFnDocument(string path)
        {
            if(File.Exists(path))
            {
                string fileContent = string.Empty;
                using (StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path), Encoding.UTF8))
                {
                    fileContent = sr.ReadToEnd();
                }
                return _TryConvert(fileContent /*, AppSettings.JsonHeaders[0]*/ );

            }

            return FnReadedDocument.EmptyFD;
        }


        static int readed = 0;
        public static int ReadedProgress { get => readed; }
        public static List<FnReadedDocument> ReadDocuments(string[] files)
        {
            readed = 0;
            var fnReadedDocuments = new List<FnReadedDocument>();
            foreach (string file in files)
            {
                string fileContent = string.Empty;

                LogHandle.ol(Environment.NewLine+ "****************************************"+ Environment.NewLine+ "Обрабатываем файл: "+file+Environment.NewLine+ "****************************************");

                try
                {
                    using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
                    {
                        fileContent = sr.ReadToEnd();
                    }
                }
                catch(Exception exc)
                {
                    LogHandle.ol("При чтении файла произошла ошибка");
                    LogHandle.ol(exc.Message);
                    continue;
                }
                try
                {
                    LogHandle.ol("Поиск ФД в корне файла");
                    JObject json = JObject.Parse(fileContent);
                    FnReadedDocument fd = _ReadFd(json);
                    if (!fd.Equals(FnReadedDocument.EmptyFD))
                    {
                        fnReadedDocuments.Add(fd);
                    }
                }
                catch (Exception exc)
                {
                    LogHandle.ol(exc.Message);
                }
                foreach(var s in AppSettings.JsonHeaders)
                {
                    LogHandle.ol("Ищем ФД по пути "+s);
                    string[] partsPath = s.Split('.');
                    int pathIndex = 0;
                    
                    try
                    {
                        JArray ja;
                        JObject jt;
                        if (partsPath[0] == "[]")
                        {
                            ja = JArray.Parse(fileContent);
                            pathIndex++;
                            int arrayLenth = ja.Count;
                            for(int i = 0; i < arrayLenth; i++)
                            {
                                GetAccessToCheque(ja[i], pathIndex, partsPath, ref fnReadedDocuments);
                            }
                        }
                        else
                        {
                            jt = JObject.Parse(fileContent);

                            GetAccessToCheque(jt, pathIndex, partsPath, ref fnReadedDocuments);
                        }

                    }
                    catch(Exception exParce)
                    {
                        LogHandle.ol(exParce.Message);
                        continue;
                    }               
                }

                readed++;
            }

            return fnReadedDocuments;
        }

        
        static void GetAccessToCheque(JToken token, int pathIndex, string[] path, ref List<FnReadedDocument> collector)
        {
            if(pathIndex == path.Length)
            {
                LogHandle.ol("достигнут конец пути");
                try
                {
                    JObject jsCheque = JObject.Parse(token.ToString());
                    FnReadedDocument fd = _ReadFd(jsCheque);
                    if (!fd.Equals(FnReadedDocument.EmptyFD))
                    {
                        collector.Add(fd);
                    }

                }
                catch(Exception ex)
                {
                    LogHandle.ol(ex.Message);   
                }
                return;
            }

            string pathElem = path[pathIndex++];
            if (pathElem.EndsWith("[]"))
            {
                string pathElemCurrent = pathElem.Substring(0, pathElem.Length - 2);
                JToken tokenNextStep = token.SelectToken(pathElemCurrent);
                if (tokenNextStep == null)
                    return;
                int tokensCount = tokenNextStep.Children().Children().Count();
                for(int i = 0; i < tokensCount; i++)
                {
                    GetAccessToCheque(tokenNextStep.Children().Children().ToList<JToken>()[i], pathIndex, path, ref collector);
                }
            }
            else
            {
                JToken tokenNextStep = token.SelectToken(pathElem);
                if(tokenNextStep!=null)
                    GetAccessToCheque(tokenNextStep, pathIndex, path, ref collector);
            }

        }


        public static FnReadedDocument _TryConvert(string json, string header = "")
        {
            try
            {
                JObject jc = null;
                JArray ja = null;
                string pathTmp = header.Trim();
                if (pathTmp.StartsWith("[]"))
                {
                    ja = JArray.Parse(json);
                }
                else
                {
                    jc = JObject.Parse(json);
                }
                string[] paths = pathTmp.Split('.');


                return _ReadFd(jc);
            }
            catch(Exception ex)
            {
                LogHandle.ol(ex.StackTrace);
                LogHandle.ol(ex.Message);
            }
            return FnReadedDocument.EmptyFD;
        }

        public static FnReadedDocument _ReadFd(JObject chequeContent)
        {
            LogHandle.ol("Разбор json документа: "+chequeContent.ToString().Replace(Environment.NewLine,""));
            FTag root = null;
            try
            {
                int jsonPathsInFd = chequeContent.Count;
                List < FTag > ftagContainer = new List<FTag>();
                JToken token = chequeContent.First;
                foreach(JToken jt in chequeContent.Children())
                {
                    if(jt.Path == "code")
                    {
                        LogHandle.ol("__token: " + jt.ToString()+ " FD root tag");
                        int documentName = jt.First.Value<int>();
                        root = new FTag(documentName, ftagContainer, false);
                    }
                    else if(jt.Path == "items"|| jt.Path == FTag.fnsNames[FTAG_ITEM])
                    {
                        LogHandle.ol("parce items____");
                        int itemNumbers = jt.Children().Children().Count();

                        for(int i=0; i < itemNumbers; i++)
                        {
                            LogHandle.ol("_item_[" + i+"]");
                            List<string[]> stringsReplaceRule = new List<string[]> { new string[] {"items["+i+"]","items" } };
                            if (FTag.rulesChanged)
                            {
                                stringsReplaceRule = new List<string[]> { new string[] { FTag.fnsNames[FTAG_ITEM] +"[" + i + "]", FTag.fnsNames[FTAG_ITEM] } };
                            }

                            List<FTag> itemTagsList = new List<FTag>();
                            foreach (JToken jtItemCh in jt.Children()[i].Children())
                            {
                                itemTagsList.Add(GetFTagOfJsProperty(jtItemCh, stringsReplaceRule));
                                LogHandle.ol(jtItemCh.ToString());
                            }
                            
                            ftagContainer.Add(new FTag(FTAG_ITEM, itemTagsList, false));
                            LogHandle.ol(ftagContainer[ftagContainer.Count - 1].ToString());
                        }
                    }
                    else if(jt.Path == FTag.fnsNames[FTAG_AMOUNTS_RECEIPT_NDS])
                    {
                        LogHandle.ol("parce amounnts nds____");
                        JToken amounts = jt.Children().Children().First();
                        LogHandle.ol(amounts.Path);
                        int amountNdsSumsCount = amounts.Children().Children().Count();
                        
                        List<FTag> famouny = new List<FTag>();
                        
                        FTag fAmountsNdsSums = new FTag(FTAG_AMOUNTS_RECEIPT_NDS, famouny,false); 
                        for(int i = 0; i < amountNdsSumsCount; i++)
                        {
                            FTag fAmountNds = new FTag(FTAG_AMOUNTS_NDS, new List<FTag>(), false);
                            LogHandle.ol("_amountsNds_[" + i + "]");
                            List<string[]> stringsReplaceRule = new List<string[]> { new string[] { "amountsNds[" + i + "]", "amountsNds" } };
                            if (FTag.rulesChanged)
                            {
                                stringsReplaceRule = new List<string[]> { new string[] { FTag.fnsNames[FTAG_AMOUNTS_NDS] + "[" + i + "]", FTag.fnsNames[FTAG_AMOUNTS_NDS] } };
                            }

                            List<FTag> itemTagsList = new List<FTag>();
                            foreach (JToken jtIAmountCh in amounts.Children()[i].Children())
                            {
                                fAmountNds.Nested.Add(GetFTagOfJsProperty(jtIAmountCh, stringsReplaceRule));
                                LogHandle.ol(jtIAmountCh.ToString());
                            }
                            famouny.Add(fAmountNds);
                            //ftagContainer.Add(new FTag(FTAG_ITEM, itemTagsList, false));
                            //LogHandle.ol(ftagContainer[ftagContainer.Count - 1].ToString());

                        }
                        ftagContainer.Add(fAmountsNdsSums);

                        LogHandle.ol(amounts.ToString());
                    }
                    else
                    {
                        LogHandle.ol("__token: " + jt.ToString());
                        if(jt.ToString().Contains("\"taxationType\""))
                        {
                            LogHandle.ol("pofd taxation type fix");
                            object fs = jt.First;
                            var tmpTag = new FTag(FTAG_APPLIED_TAXATION_TYPE, fs, false);
                            if(tmpTag.TagNumber== FTAG_APPLIED_TAXATION_TYPE)
                            {
                                ftagContainer.Add(tmpTag);
                            }
                        }
                        ftagContainer.Add(GetFTagOfJsProperty(jt));
                    }

                }

                if(root == null && FTag.rulesChanged && FTag.DefaultFormCode > 0)
                {
                    // для кастомных выгрузок
                    root = new FTag(FTag.DefaultFormCode, ftagContainer, false);
                }

                
                if (root == null)
                {
                    LogHandle.ol("В разбираемом токене отсутсвует корневой тег(номер формы ФД)");
                    return FnReadedDocument.EmptyFD;
                }
                    
                root.RebuildPrezentation();
                
                FnReadedDocument parsedJson = TranslateFtagsList(new byte[] { }, root.TagNumber, new List<FTag>() { root });
                if (!parsedJson.Equals(FnReadedDocument.EmptyFD) && parsedJson.Cheque != null)
                {
                    if (
                        parsedJson.Cheque.DocumentNameFtagType == FTAG_FISCAL_DOCUMENT_TYPE_BSO ||
                        parsedJson.Cheque.DocumentNameFtagType == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE ||
                        ((parsedJson.Cheque.DocumentNameFtagType == FD_DOCUMENT_NAME_CORRECTION_CHEQUE || parsedJson.Cheque.DocumentNameFtagType == FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION)&&AppSettings.OverrideCorrectionDocumentDate)
                        )
                    {
                        parsedJson.Cheque.CorrectionDocumentDate = parsedJson.Time.Date;
                    }
                    if ( AppSettings.AppendFiscalSignAsPropertyData
                    && (!parsedJson.Cheque.IsPropertiesData || AppSettings.OverridePropertyData)
                    && !string.IsNullOrEmpty(parsedJson.FiscalSign) )
                    {
                        parsedJson.Cheque.PropertiesData = parsedJson.FiscalSign;
                    }
                }
                return parsedJson;

            }
            catch(Exception exc)
            {
                LogHandle.ol(exc.ToString());
            }
            return FnReadedDocument.EmptyFD;
        }

        static FTag GetFTagOfJsProperty(JToken jt, List<string[]>replaceRules = null)
        {
            string path = jt.Path;
            if(replaceRules != null)
            {
                foreach(string[] replaceRule in replaceRules)
                {
                    if(replaceRule.Length > 1)
                    {
                        path = path.Replace(replaceRule[0], replaceRule[1]);
                    }
                }

            }

            int tagNumber = -1;
            
            if (FTag.structuredTagNames.ContainsKey(path))
                tagNumber = FTag.structuredTagNames[path];
            else
            {
                LogHandle.ol("unknown tag name: " + path+Environment.NewLine+ "skip this entry");

                return FTag.Empty;
            }
            FTag tag = null;
            FTag.FDataType ftagType = FTag.typeMap[tagNumber];
            if (
                ftagType == FTag.FDataType.Bit_MASK ||
                ftagType == FTag.FDataType.Uint16 ||
                ftagType == FTag.FDataType.Uint32 ||
                ftagType == FTag.FDataType.NUMBER ||
                ftagType == FTag.FDataType.BYTE
                )
            {
                int value = (int)jt.First;
                tag = new FTag(tagNumber, value, false);
            }
            else if (ftagType == FTag.FDataType.Byte_ARRAY)
            {
                if(tagNumber == FTAG_DOC_FISCAL_SIGN)
                {
                   
                    object fs = jt.First;
                    tag = new FTag(tagNumber, fs, false);
                }
            }
            else if (ftagType == FTag.FDataType.U32UT)
            {
                DateTime dt = new DateTime(1970,1,1,0,0,0);

                try
                {
                    IFormatProvider prov = CultureInfo.InvariantCulture;
                    var s = jt.First.ToString();
                    DateTimeStyles st = DateTimeStyles.None;
                    if (!DateTime.TryParseExact(s, "dd/MM/yyyy HH:mm", prov, st, out dt))
                        if (!DateTime.TryParseExact(s, "dd-MM-yyyy HH:mm", prov, st, out dt))
                            if (!DateTime.TryParseExact(s, "dd.MM.yyyyTHH:mm", prov, st, out dt))
                                if (!DateTime.TryParseExact(s, "dd.MM.yyyy HH:mm", prov, st, out dt))
                                    if (!DateTime.TryParseExact(s, "dd.MM.yyyy", prov, st, out dt))
                                        if (!DateTime.TryParseExact(s, "yyyy.MM.dd HH:mm", prov, st, out dt))
                                            if (!DateTime.TryParseExact(s, "yyyy.MM.dd HH:mm:ss", prov, st, out dt))
                                                if (!DateTime.TryParseExact(s, "yyyy/MM/dd HH:mm", prov, st, out dt))
                                                    if (!DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm", prov, st, out dt))
                                                        if (!DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", prov, st, out dt))
                                                            if (!DateTime.TryParseExact(s, "O", prov, st, out dt))
                                                                dt = jt.First.Value<DateTime>();
                }
                catch { }
                if(dt.Year > 2017)
                    tag = new FTag(tagNumber, dt, false);
                else
                {
                    string strDateTime = (string)jt.First;
                    tag = new FTag(tagNumber, strDateTime, false);
                }
                
            }
            else if (ftagType == FTag.FDataType.VLN)
            {
                long price = (long)jt.First;
                tag = new FTag(tagNumber, price, false);
            }
            else if (ftagType == FTag.FDataType.FVLN)
            {
                double quantity = (double)jt.First;
                tag = new FTag(tagNumber, quantity, false);
            }
            else if (ftagType == FTag.FDataType.STRING)
            {
                 //string s = (string)jt.First;
                 tag = new FTag(tagNumber, jt.First.ToString(), false);
            }
            else if (ftagType == FTag.FDataType.STLV)
            {

                if(tagNumber == FTAG_ITEM)
                {
                    /* 
                     * варианты
                     * 1 обработка этого тега должна проходить на более высоком уровне
                     * 1 + передавать сюда по одному итемсу перебирая массив
                     * 2 либо добавлять оверрайд метод возвращающий список или массив тегов
                     * 
                     *  ********
                     *  сделано - сюда попадать не должны
                     * 
                     */
                    LogHandle.ol("Проблема с разбором. Возможно проблема с правилами разбора, необходим анализ кода.");
                    return FTag.Empty;

                }
                List<FTag> tagsIncluded = new List<FTag>();
                foreach(JToken gtChild in jt.Children().Children())
                {
                    tagsIncluded.Add(GetFTagOfJsProperty(gtChild,replaceRules));
                }

                tag = new FTag(tagNumber, tagsIncluded, false);

            }
            LogHandle.ol(tag.ToString());
            return tag;
        }




        public static string CreateJsonString(FnReadedDocument fd)
        {
            JObject jsfd = new JObject();
            JArray jaItems = new JArray();
            JArray amountsNds = new JArray();
            if(fd.Tags==null || fd.Tags.Count == 0)
            {
                if (fd.Cheque != null)
                {
                    List<FTag> ftags = fd.Cheque.CreateTask();
                    ftags.Add(new FTag(FTAG_DATE_TIME, fd.Time, false));
                    ftags.Add(new FTag(FTAG_TOTAL_SUM, (int)Math.Round(fd.Summ*100), false));
                    ftags.Add(new FTag(FTAG_FD, fd.Number, false));
                    FTag fdCreated = new FTag(fd.Type, ftags, false);
                    fd.Tags = new List<FTag>();
                    fd.Tags.Add(fdCreated);
                }

            }

            if (fd.Tags != null && fd.Tags.Count > 0)
            {
                foreach(FTag tag in fd.Tags)
                {
                    SaveTagAsJsonProperty(tag, ref jsfd, ref jaItems, ref amountsNds);
                }
            }
            else
            {
                LogHandle.ol("Метод чтения ФД не предусматривает возможности сохранения в JSON формате");
            }
            LogHandle.ol(jsfd.ToString());
            return jsfd.ToString();
        }

        private static void SaveTagAsJsonProperty(FTag tag, ref JObject jsfd, ref JArray items, ref JArray amountsNds)
        {
            string tagName = FTag.fnsNames.ContainsKey(tag.TagNumber)?FTag.fnsNames[tag.TagNumber]:null;
            if (tag.TagNumber < 100)
                tagName = "code";

            if (tagName == null)
            {
                
                LogHandle.ol("Не найдено название тега в словаре пропускаем обработку тега");
                return;
            }
            switch (tag.Type)
            {
                case FTag.FDataType.STLV:
                    if(tagName == "code")
                    {
                        jsfd.Add("code", tag.TagNumber);
                        foreach(FTag t in tag.Nested)
                            SaveTagAsJsonProperty(t, ref jsfd,ref items, ref amountsNds);
                    }
                    else
                    {
                        JObject jt = new JObject();
                        foreach (FTag t in tag.Nested)
                        {
                            SaveTagAsJsonProperty(t, ref jt,ref items, ref amountsNds);
                        }
                        if (tag.TagNumber != FTAG_ITEM && tag.TagNumber != FTAG_AMOUNTS_NDS)
                            jsfd.Add(tagName, jt);
                        else
                        {
                            if(tag.TagNumber == FTAG_ITEM)
                            {
                                items.Add(jt);
                                if (items.Count == 1)
                                    jsfd.Add("items", items);
                            }
                            else if(tag.TagNumber == FTAG_AMOUNTS_NDS)
                            {
                                amountsNds.Add(jt);
                                if (amountsNds.Count == 1)
                                {
                                    jsfd.Add(FTag.fnsNames[FTAG_AMOUNTS_NDS] , amountsNds);

                                }
                                    
                            }
                            else if (tag.TagNumber == FTAG_AMOUNTS_RECEIPT_NDS)
                            {
                                // тут пропускаем
                            }
                            
                        }
                    }
                    break;
                case FTag.FDataType.FVLN:
                    jsfd.Add(tagName,tag.ValueDouble);
                    break;
                case FTag.FDataType.VLN:
                    jsfd.Add(tagName, (long)Math.Round(tag.ValueDouble*100));
                    break;
                case FTag.FDataType.STRING:
                    jsfd.Add(tagName, tag.ValueStr);
                    break;
                case FTag.FDataType.Bit_MASK:
                case FTag.FDataType.NUMBER:
                case FTag.FDataType.Uint16:
                case FTag.FDataType.Uint32:
                case FTag.FDataType.BYTE:
                    jsfd.Add(tagName, tag.ValueInt);
                    break;
                case FTag.FDataType.U32UT:
                    jsfd.Add(tagName,tag.ValueDT.ToString(DEFAULT_DT_FORMAT));
                    break;
                case FTag.FDataType.Byte_ARRAY:
                    if(tag.TagNumber == FTAG_DOC_FISCAL_SIGN)
                    {
                        long x =-1;
                        if (long.TryParse(tag.ValueStr, out x))
                        {
                            jsfd.Add(tagName, tag.ValueStr);
                        }
                        else if (tag.ValueInt > 0)
                        {
                            jsfd.Add(tagName, tag.ValueInt);
                        }
                    }
                    break;
            }

        }
    }
}
