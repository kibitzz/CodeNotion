using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Org.BouncyCastle.Ocsp;
using System.Collections.Concurrent;

namespace basicClasses.models.Roslyn
{

    [appliable("func Action")]
    [info("filler.")]
    public class syntax_tree : ModelBase
    {
        [info("")]
        [model("")]
        public static readonly string Solution = "Solution";

        [info("")]
        [model("")]
        public static readonly string Project = "Project";

        [info("")]
        [model("")]
        public static readonly string OnError = "OnError";

        opis onErr = null;

        static bool isInitialized = false;
        static string[] parr = new string[] { "Expression", "Identifier", "Text", "Name", "Keyword", "Token" };
        ConcurrentDictionary<string, string> TypeNames;

        int threadMax = 120;

        static Dictionary<string, string> CachedNames = new Dictionary<string, string>();

        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();
            onErr = ms.getPartitionNotInitOrigName(OnError);

            var rez = new opis();
            int rezcou = 0;

            TypeNames = new ConcurrentDictionary<string, string>(threadMax, 2000);

            if (!isInitialized)
            MSBuildLocator.RegisterDefaults();

            isInitialized = true;

            var wsp = MSBuildWorkspace.Create();
            wsp.WorkspaceFailed += handleError;


            if (ms.isHere(Solution))
            {
                var sol = wsp.OpenSolutionAsync(ms.V(Solution));
                sol.Wait();
                var s = sol.Result;

                dosmth(s, rez).Wait();                
            }


            if (ms.isHere(Project))
            {
                var proj = wsp.OpenProjectAsync(ms.V(Project));
                proj.Wait();
                var p = proj.Result;
            }


            message.CopyArr(rez);


        }

        public async Task dosmth(Solution s, opis r)
        {
            int threadMax = 120;
           
            var tasks = new List<Task>();

            foreach (var p in s.Projects)
            {
                opis po = new opis(3) {PartitionName = p.Name, PartitionKind = "project" };
                po["arrt"].Vset("DefaultNamespace", p.DefaultNamespace);
                r.AddArr(po);

                tasks.Add(dosmth(p, po));
                //await dosmth(p, po);

                if (tasks.Count == threadMax)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }
        }

        public async Task dosmth(Project p, opis r)
        {
            opis docs = r["documents"];

            var compil = await p.GetCompilationAsync();
            if (compil is null) return;
            
            var tasks = new List<Task>();

            foreach (var doc in p.Documents)
            {              
                var d = new opis(3) { PartitionName = doc.Name, PartitionKind = "document" };
                d["attr"].Vset("Folders", String.Join("/", doc.Folders));
                d["attr"].Vset("FilePath", doc.FilePath);
                
                docs.AddArr(d);

                #region paralel
                //tasks.Add(ProcProjectDocument(compil, doc, d));

                //if (tasks.Count == threadMax)
                //{
                //    await Task.WhenAll(tasks);
                //    tasks.Clear();
                //}

                #endregion paralel

                var tree = await doc.GetSyntaxTreeAsync();
                if (tree is null) continue;

                var semant = compil.GetSemanticModel(tree, true);
                var root = tree.GetRoot();

                ProcSyntaxTreeNode(root, semant, d["tree"]);


                //  tree.ChildNodes().OfType<MethodDeclarationSyntax>();
                //tree.ChildNodes();
            }
        }

        public async Task ProcProjectDocument(Compilation compil, Document doc, opis d)
        {
            var tree = await doc.GetSyntaxTreeAsync();
            if (tree is null) return;

            var semant = compil.GetSemanticModel(tree, true);
            var root = tree.GetRoot();

            ProcSyntaxTreeNode(root, semant, d["tree"]);
        }

        void ProcSyntaxTreeNode(SyntaxNode sn, SemanticModel semant, opis o)
        {
            var nodetype = sn.GetType();
            o.PartitionKind = GetTypeName(nodetype);

            var declarationInfo = semant.GetDeclaredSymbol(sn);
          //  var symbolInfo = semant.GetSymbolInfo(sn);
            var typeInfo = semant.GetTypeInfo(sn);

            //  var nodeOperation = semant.GetOperation(sn);
            // nodeOperation.Kind


            //  var semanttype = typeInfo.Type; // can be null
            // var definedInLocations = nodesymbol.Locations;

           
            if ( typeInfo.Type != null)
            {                
                o.Vset("typ", typeInfo.Type.ToString());
            }
                      


            int propcou =0;

            void TryGetProperty(string propName)
            {
                if (propcou > 0) return;
                var propInfo = nodetype.GetProperty(propName);
                if (propInfo is null) return;
                var prop = propInfo.GetValue(sn, null);
                if (prop is null) return;
                
                propcou++;
                o.PartitionName += GetName(prop.ToString());
            }

            foreach (var p in parr)
                TryGetProperty(p);

            var sub = new opis();

            foreach (var n in sn.ChildNodes())
            {
                var on = new opis();
                sub.AddArr(on);

                ProcSyntaxTreeNode(n, semant, on);
            }

            if (o.PartitionName == null || o.PartitionName.Length > 140)
                o.PartitionName = string.Empty;

            if (declarationInfo != null && declarationInfo.Kind == SymbolKind.NamedType)
            {
                o.Vset("decl", declarationInfo.ContainingSymbol.ToString() + "." + o.PartitionName);
            }

            //if (propcou > 0)
            //    o["sub"] = sub;
            //else
            o.AddArrRange(sub);



            // sn.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.)
            // sn.
        }



        string GetTypeName(Type t)
        {
            var tn = t.Name;
            if (TypeNames.TryGetValue(tn, out var name))
            {
                return name;
            }
            else
            {
                var rez = tn.Substring(0, tn.Length > 6 ? tn.Length - 6 : tn.Length);               
                TypeNames.TryAdd(tn, rez);      
                
                return rez;
            }
        }

        string GetName(string tn)
        {
            return tn;

            if (CachedNames.TryGetValue(tn, out var name))
            {
                return name;
            }
            else
            {
                var rez = tn;
                try
                {
                    CachedNames.Add(tn, rez);
                } 
                catch { }
                return rez;
            }
        }

        //void Locations(locations)
        //{
        //    var loc = new opis();
        //    o["locations"] = loc;
        //    foreach (var l in locations)
        //    {
        //        var linespan = l.GetLineSpan();
        //        loc.AddArr(new opis(0)
        //        {
        //            PartitionKind = l.Kind.ToString(),
        //            PartitionName = linespan.Path + " " + linespan.StartLinePosition
        //        });
        //    }
        //}

        string GetIdentifierType(ISymbol nodesymbol)
        {
            return nodesymbol.ContainingNamespace + "." + nodesymbol.ContainingSymbol;
        }

        public void handleError(object sender, WorkspaceDiagnosticEventArgs e)
        {
            if (onErr != null)
            {
                var oe = new opis() { PartitionName = "exception", body = e.ToString() };
                instanse.ExecActionResponceModelsList(onErr, oe);
            }
        }
    }

}
