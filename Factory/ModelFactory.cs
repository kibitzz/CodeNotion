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
using basicClasses.models.Roslyn;
using basicClasses.models.svcutil;
using basicClasses.models.file_system;
using basicClasses.models.deserializer;

namespace basicClasses.Factory
{
   public class ModelFactory
    {
        Dictionary<string, ModelInfo> models;

        public static opis hotkeys = new opis();
        public static opis hotkeys_mod = new opis();

        public static Dictionary<string, ModelBase> ExternalModels;

        public ModelFactory()
        {
            Init();
        }

        public void Init()
        {
            models = new Dictionary<string, ModelInfo>
            {
                { "valf", new valf() },
                { "envf", new envf() },
                { "Messenger", new Messenger() },
                { "AnswerToMessage", new AnswerToMessage() },
                { "initValues", new initValues() },
                { "ConditionChecker", new ConditionChecker() },

                { "fill_Role", new fill_Role() },// chorter name is better
                { "fill_shared_context_Role", new fill_shared_context_Role() },//obsolete
                { "GetAnyPartOfOpis", new GetAnyPartOfOpis() },
                { "BodyValueModificator", new BodyValueModificator() },


                { "Breaker", new Breaker() },
                { "RangeAndAssign", new RangeAndAssign() },
                { "ForEach", new ForEach() },
                { "exec", new exec() },
                { "exec_inline", new exec_inline() },

                { "TreeDataExtractor", new TreeDataExtractor() },//obsolete
                { "Destructure", new Destructure() },

                { "EditingPart", new EditingPart() },

                { "MsgTemplate", new MsgTemplate() },
                { "Pause_To_debug", new Pause_To_debug() },
                { "global_log", new global_log() },
                { "local_log", new local_log() },
                { "sys_log_info", new sys_log_info() },
                { "UserNotifier", new UserNotifier() },


                { "Fill_org", new Fill_org() },
                { "Fill_org_upper", new Fill_org_upper() },
                { "Fill_upper_context", new Fill_upper_context() },
                { "Fill_current_context", new Fill_current_context() },


                { "context", new context() },
                { "Fill_Spec_Info", new Fill_Spec_Info() },


                { "MoveToSharedVarInstCont", new MoveToSharedVarInstCont() },
                { "fill_Context_info", new fill_Context_info() },
                { "ConditionResponceModel", new ConditionResponceModel() },
                { "ClearSharedContext", new ClearSharedContext() },
                { "SharedContextRoles", new SharedContextRoles() },
                { "SubContextHandler", new SubContextHandler() },


                { "TagsTypes", new TagsTypes() },
                { "SysInst_contextsArray", new SysInst_contextsArray() },
                { "ContextHierarchyTypes", new ContextHierarchyTypes() },

                { "ModelBase", new ModelBase() },
                { "ModelNotion", new ModelNotion() },
                { "modelReq", new modelReq() },
                { "ModelListOfMessages", new ModelListOfMessages() },
                { "Messaging", new Messaging() },

                { "targetWholeBranch", new targetWholeBranch() },
                { "targetUpper", new targetUpper() },
                { "targetUpperOnly", new targetUpperOnly() },
                { "targetSubTree", new targetSubTree() },
                { "targetSameCont", new targetSameCont() },
                { "TargetingChecks", new TargetingChecks() },
                { "getInstContexts", new getInstContexts() },
                { "check_Conformity", new check_Conformity() },
                { "targetSubcontextsOfItems", new targetSubcontextsOfItems() },
                { "targetAnyCont", new targetAnyCont() },
                { "targetSubTreeOnly", new targetSubTreeOnly() },
                { "targetParentCont", new targetParentCont() },


                { "fill_contex_Stub", new fill_contex_Stub() },
                { "buildTreeVal_sdc_i", new buildTreeVal_sdc_i() },

                { "spec_tag", new spec_tag() },
                { "RangeMax", new RangeMax() },
                { "RangeMin", new RangeMin() },
                { "RangingList", new RangingList() },
                { "GetWordForm", new GetWordForm() },
                { "GenerateID", new GenerateID() },

                { "load_data_file", new load_data_file() },
                { "Check_list_more_1", new Check_list_more_1() },
                { "Check_list_more_0", new Check_list_more_0() },
                { "Check_Key_value_prsnce", new Check_Key_value_prsnce() },
                { "file_formats", new file_formats() },
                { "jsonParserModel", new jsonParserModel() },
                { "headersParserModel", new headersParserModel() },
                { "webActorModel", new webActorModel() },
                { "ConcPostDataFormat", new ConcPostDataFormat() },
                { "save_toFile", new save_toFile() },
                { "AccountsParserModel", new AccountsParserModel() },
                { "Opis_ParserModel", new Opis_ParserModel() },
                { "SimpleLines_ParserModel", new SimpleLines_ParserModel() },
                { "Fill_templare_from_dataSouce", new Fill_templare_from_dataSouce() },
                { "upLoadFileSpecs", new upLoadFileSpecs() },
                { "Check_body_isFilled", new Check_body_isFilled() },
                { "numArrNames", new numArrNames() },

                { "PutTimestamp", new PutTimestamp() },
                { "Check_TimestampDiff", new Check_TimestampDiff() },
                { "TagArrayItems", new TagArrayItems() },
                { "NextArrItm", new NextArrItm() },
                { "TemplateSearch", new TemplateSearch() },
                { "EmbedTree", new EmbedTree() },

                { "Make_pause", new Make_pause() },
                { "Check_isInArray", new Check_isInArray() },
                { "Check_is_odd", new Check_is_odd() },
                { "Check_contain_string", new Check_contain_string() },

                { "boardsset_ParserModel", new boardsset_ParserModel() },
                { "Compress", new Compress() },
                { "cleaner", new cleaner() },
                { "BuildTermBasicValuesContainer", new BuildTermBasicValuesContainer() },
                { "SyntaxTreeMatchIdPatterns", new SyntaxTreeMatchIdPatterns() },
                { "ClearStruct", new ClearStruct() },
                { "DateTimeComp", new DateTimeComp() },
                { "CompareFill", new CompareFill() },
                { "Parce_Body_as_Json", new Parce_Body_as_Json() },
                { "opis_to_json", new opis_to_json() },

                { "CheckOverrides", new CheckOverrides() },



                { "jint", new jint() },
                { "HtmlParser_HAP", new HtmlParser_HAP() },
                { "UseSecurityProtocol", new UseSecurityProtocol() },
                { "code_point", new code_point() },


                { "targetID", new targetID() },
                { "seqreader", new seqreader() },
                { "GetEnclosedText", new GetEnclosedText() },
                { "WordsArrOp", new WordsArrOp() },
                { "StrReplace", new StrReplace() },


                { "universal_line_pareser", new universal_line_pareser() },
                { "table_parser", new table_parser() },
                { "SingleLineText_ParserModel", new SingleLineText_ParserModel() },
                { "MySqlQuery", new MySqlQuery() },
                { "sql_server_query", new sql_server_query() },

                { "CompDataExport", new CompDataExport() },
                { "FindCommonDatastructure", new FindCommonDatastructure() },
                { "send_mail", new send_mail() },
                { "smtp_settings", new smtp_settings() },
                { "Ftp_client", new Ftp_client() },
                { "ftp_settings", new ftp_settings() },


                { "Data_In_Buffer", new Data_In_Buffer() },
                { "PeriodIterator", new PeriodIterator() },
                { "GlobalParamsProvider", new GlobalParamsProvider() },

                { "IntactCopyChecker", new IntactCopyChecker() },
                { "thread_lock", new thread_lock() },
                { "arr_chuncks", new arr_chuncks() },
                { "text_case_transform", new text_case_transform() },
                { "b_length", new b_length() },
                { "arr_length", new arr_length() },
                { "reverse_order", new reverse_order() },
                { "order_by", new order_by() },
                { "proc_recursive", new proc_recursive() },

                { "normalize_spaces", new normalize_spaces() },
                { "linear_chart", new linear_chart() },
                { "garbage_collection", new garbage_collection() },

                { "NodesPathInfo", new NodesPathInfo() },
                { "TextCodec", new TextCodec() },
                { "RegexParcer", new RegexParcer() },
                { "HttpListenerModel", new HttpListenerModel() },
                { "car_cdr_oper", new car_cdr_oper() },
                { "file", new file() },
                { "fuzzy_search", new fuzzy_search() },
                { "syntax_tree", new syntax_tree() },
                { "clear_stack_itms", new clear_stack_itms() },
                { "mary_player_service", new mary_player_service() },

                { "file_tree_scanner", new file_tree_scanner() },
                { "dir_file_info", new dir_file_info() }
                
            };



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
            if (!string.IsNullOrEmpty(name) && models.TryGetValue(name.Trim(), out m))
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
            if (!string.IsNullOrEmpty(name) && models.TryGetValue(name.Trim(), out m))
            {
                return m;
            }
            else
                return null;
        }

        public opis GetModelInfo(string name)
        {
            ModelInfo m;
            if (!string.IsNullOrEmpty(name) && models.TryGetValue(name.Trim(), out m))
            {
                return m.Info;
            }
            else
            {
                opis rez = new opis();            

                return rez;
            }
        }

       
    }
}
