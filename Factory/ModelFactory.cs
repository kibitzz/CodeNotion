using basicClasses.models;
using basicClasses.models.Extractors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using basicClasses.models.Actions;
using basicClasses.models.OptionsLists;

using basicClasses.models.FunctionalInstances;
using basicClasses.models.Builders;
using basicClasses.models.ContextOperators;
using basicClasses.models.SharedDataContextDrivers;
using basicClasses.models.DataSets;
using basicClasses.models.StructureProcessing;
using basicClasses.models.sys_ext;
using basicClasses.models.Checkers;
using basicClasses.models.Rangers;
using basicClasses.models.WEB_api;
using basicClasses.models.String_proc;
using basicClasses.models.SQL;
using basicClasses.models.Markers;

namespace basicClasses.Factory
{
   public class ModelFactory
    {
        Dictionary<string, ModelInfo> models;

        public static opis hotkeys = new opis();

        public static Dictionary<string, ModelBase> ExternalModels;

        public ModelFactory()
        {
            Init();
        }

        public void Init()
        {
            models = new Dictionary<string, ModelInfo>();
           
            models.Add("valf", new valf());
            models.Add("envf", new envf());
            models.Add("Messenger", new Messenger());
            models.Add("AnswerToMessage", new AnswerToMessage());
            models.Add("initValues", new initValues());
            models.Add("ConditionChecker", new ConditionChecker());

            models.Add("fill_Role", new fill_Role());// chorter name is better
            models.Add("fill_shared_context_Role", new fill_shared_context_Role());//obsolete
            models.Add("GetAnyPartOfOpis", new GetAnyPartOfOpis());
            models.Add("BodyValueModificator", new BodyValueModificator());


            models.Add("Breaker", new Breaker());
            models.Add("RangeAndAssign", new RangeAndAssign());
            models.Add("ForEach", new ForEach());
            models.Add("exec", new exec());
            models.Add("exec_inline", new exec_inline());            

            models.Add("TreeDataExtractor", new TreeDataExtractor());//obsolete
            models.Add("Destructure", new Destructure());            

            models.Add("EditingPart", new EditingPart());

            models.Add("MsgTemplate", new MsgTemplate());
            models.Add("Pause_To_debug", new Pause_To_debug());
            models.Add("global_log", new global_log());
            models.Add("local_log", new local_log());
            models.Add("sys_log_info", new sys_log_info());            
            models.Add("UserNotifier", new UserNotifier());


            models.Add("Fill_org", new Fill_org());
            models.Add("Fill_org_upper", new Fill_org_upper());
            models.Add("Fill_upper_context", new Fill_upper_context());
            models.Add("Fill_current_context", new Fill_current_context());
                     
        
            models.Add("context", new context());
            models.Add("Fill_Spec_Info", new Fill_Spec_Info());                           
          
            
            models.Add("MoveToSharedVarInstCont", new MoveToSharedVarInstCont());                  
            models.Add("fill_Context_info", new fill_Context_info());                                
            models.Add("ConditionResponceModel", new ConditionResponceModel());           
            models.Add("ClearSharedContext", new ClearSharedContext());        
            models.Add("SharedContextRoles", new SharedContextRoles());
            models.Add("SubContextHandler", new SubContextHandler());
            
           
            models.Add("TagsTypes", new TagsTypes());          
            models.Add("SysInst_contextsArray", new SysInst_contextsArray());         
            models.Add("ContextHierarchyTypes", new ContextHierarchyTypes());
        
            models.Add("ModelBase", new ModelBase());
            models.Add("ModelNotion", new ModelNotion());
            models.Add("modelReq", new modelReq());
            models.Add("ModelListOfMessages", new ModelListOfMessages());
            models.Add("Messaging", new Messaging());

            models.Add("targetWholeBranch", new targetWholeBranch());
            models.Add("targetUpper", new targetUpper());
            models.Add("targetSubTree", new targetSubTree());
            models.Add("targetSameCont", new targetSameCont());
            models.Add("TargetingChecks", new TargetingChecks());
            models.Add("getInstContexts", new getInstContexts());
            models.Add("check_Conformity", new check_Conformity());
            models.Add("targetSubcontextsOfItems", new targetSubcontextsOfItems());
            models.Add("targetAnyCont", new targetAnyCont());
            models.Add("targetSubTreeOnly", new targetSubTreeOnly());
            models.Add("targetParentCont", new targetParentCont());
            

            models.Add("fill_contex_Stub", new fill_contex_Stub());
            models.Add("buildTreeVal_sdc_i", new buildTreeVal_sdc_i());

            models.Add("spec_tag", new spec_tag());
            models.Add("RangeMax", new RangeMax());
            models.Add("RangeMin", new RangeMin());
            models.Add("RangingList", new RangingList());
            models.Add("GetWordForm", new GetWordForm());
            models.Add("GenerateID", new GenerateID());         
                              
            models.Add("load_data_file", new load_data_file());
            models.Add("Check_list_more_1", new Check_list_more_1());
            models.Add("Check_list_more_0", new Check_list_more_0());            
            models.Add("Check_Key_value_prsnce", new Check_Key_value_prsnce());
            models.Add("file_formats", new file_formats());
            models.Add("jsonParserModel", new jsonParserModel());
            models.Add("headersParserModel", new headersParserModel());          
            models.Add("webActorModel", new webActorModel());
            models.Add("ConcPostDataFormat", new ConcPostDataFormat());
            models.Add("save_toFile", new save_toFile());
            models.Add("AccountsParserModel", new AccountsParserModel());
            models.Add("Opis_ParserModel", new Opis_ParserModel());
            models.Add("SimpleLines_ParserModel", new SimpleLines_ParserModel());            
            models.Add("Fill_templare_from_dataSouce", new Fill_templare_from_dataSouce());              
            models.Add("upLoadFileSpecs", new upLoadFileSpecs());
            models.Add("Check_body_isFilled", new Check_body_isFilled());
            models.Add("numArrNames", new numArrNames());

            models.Add("PutTimestamp", new PutTimestamp());
            models.Add("Check_TimestampDiff", new Check_TimestampDiff());
            models.Add("TagArrayItems", new TagArrayItems());
            models.Add("NextArrItm", new NextArrItm());
            models.Add("TemplateSearch", new TemplateSearch());
            models.Add("EmbedTree", new EmbedTree());
            
            models.Add("Make_pause", new Make_pause());
            models.Add("Check_isInArray", new Check_isInArray());
            models.Add("Check_is_odd", new Check_is_odd());
            models.Add("Check_contain_string", new Check_contain_string());         

            models.Add("boardsset_ParserModel", new boardsset_ParserModel());
            models.Add("Compress", new Compress());
            models.Add("cleaner", new cleaner());
            models.Add("BuildTermBasicValuesContainer", new BuildTermBasicValuesContainer());
            models.Add("SyntaxTreeMatchIdPatterns", new SyntaxTreeMatchIdPatterns());
            models.Add("ClearStruct", new ClearStruct());
            models.Add("DateTimeComp", new DateTimeComp());
            models.Add("CompareFill", new CompareFill());
            models.Add("Parce_Body_as_Json", new Parce_Body_as_Json());
            models.Add("opis_to_json", new opis_to_json());
            
            models.Add("CheckOverrides", new CheckOverrides());



            models.Add("jint", new jint());
            models.Add("HtmlParser_HAP", new HtmlParser_HAP());
            models.Add("UseSecurityProtocol", new UseSecurityProtocol());
            models.Add("code_point", new code_point());
            

            models.Add("targetID", new targetID());
            models.Add("seqreader", new seqreader());
            models.Add("GetEnclosedText", new GetEnclosedText());
            models.Add("WordsArrOp", new WordsArrOp());
            models.Add("StrReplace", new StrReplace());
            

            models.Add("universal_line_pareser", new universal_line_pareser());
            models.Add("SingleLineText_ParserModel", new SingleLineText_ParserModel());            
            models.Add("MySqlQuery", new MySqlQuery());
            models.Add("CompDataExport", new CompDataExport());
            models.Add("FindCommonDatastructure", new FindCommonDatastructure());
            models.Add("send_mail", new send_mail());
            models.Add("smtp_settings", new smtp_settings());
            models.Add("Ftp_client", new Ftp_client());
            models.Add("ftp_settings", new ftp_settings());


            models.Add("Data_In_Buffer", new Data_In_Buffer());
            models.Add("PeriodIterator", new PeriodIterator());
            models.Add("GlobalParamsProvider", new GlobalParamsProvider());

            models.Add("IntactCopyChecker", new IntactCopyChecker());
            models.Add("thread_lock", new thread_lock());
            models.Add("arr_chuncks", new arr_chuncks());
            models.Add("text_case_transform", new text_case_transform());
            models.Add("b_length", new b_length());
            models.Add("normalize_spaces", new normalize_spaces());            
            models.Add("linear_chart", new linear_chart());
            models.Add("garbage_collection", new garbage_collection());

            models.Add("NodesPathInfo", new NodesPathInfo());
            models.Add("TextCodec", new TextCodec());
            models.Add("RegexParcer", new RegexParcer());
            models.Add("HttpListenerModel", new HttpListenerModel());
           
            



            if (ExternalModels != null)
            foreach (var kvp in ExternalModels)
                models.Add(kvp.Key, kvp.Value);
        }

        public List<string> getAppliableModels(string currCont, string parentCont)
        {
            List<string> rez = new List<string>();

            foreach (var n in models.Values)
            {
              string tmp=  n.name;
                if (!string.IsNullOrEmpty(currCont) &&
                    (n.ContextsApliable.Contains(currCont)
                    && (n.ContextsApliable.Contains(parentCont)))
                    || n.ContextsApliable.Contains("all"))
                {
                    rez.Add(tmp);
                }
            }

            return rez;
        }

        public opis GetModel(string name)
        {
            ModelInfo m;
            if (models.TryGetValue(name.Trim(), out m))
            {
                opis dm = m.DataModel;

                for (int i = 0; i < dm.listCou; i++)
                {
                    if (!string.IsNullOrEmpty(dm[i].PartitionKind)
                        && dm[i].PartitionKind != name)// to avoid recursive infinite construction
                    {
                        opis pmdl = GetModel(dm[i].PartitionKind);
                        if (pmdl.isInitlze)
                            dm[dm[i].PartitionName] = pmdl;
                    }
                }

                return dm;
            }
            else
                return new opis();          
        }

        public object GetModelObject(string name)
        {
            ModelInfo m;
            if (models.TryGetValue(name.Trim(), out m))
            {
                return m;
            }
            else
                return null;
        }

        public opis GetModelInfo(string name)
        {
            ModelInfo m;
            if (models.TryGetValue(name.Trim(), out m))
            {
                return m.Info ;
            }
            else
                return new opis();
        }
    }
}
