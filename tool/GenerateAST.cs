using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.JavaScript;

namespace Tool
{
    public class GenerateAst
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: GenerateAst <output directory>");
                Environment.Exit(64);
            }

            string outputDir = args[0];

            DefineAst(
                outputDir,
                "Expr",
                new List<string>
                {
                    "Assign   : Token name, Expr value",
                    "Binary   : Expr left, Token op, Expr right",
                    "Grouping : Expr expression",
                    "Literal  : object value",
                    "Logical   : Expr left, Token op, Expr right",
                    "Unary    : Token op, Expr right",
                    "Variable : Token name"
                }
            );
            
            DefineAst(outputDir, "Stmt", new List<string>{
                "Block      : List<Stmt> statements",
                "Expression : Expr expression",
                "If         : Expr condition, Stmt thenBranch, Stmt elseBranch",
                "Print      : Expr expression",
                "Var        : Token name, Expr initializer",
                "While      : Expr condition, Stmt body"     
            });
        }

        private static void DefineAst(
            string outputDir,
            string baseName,
            List<string> types)
        {
            string path = Path.Combine(outputDir, baseName + ".cs");

            using StreamWriter writer = new(path);
            
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine();
            writer.WriteLine("namespace StarBird;");
            writer.WriteLine();
            writer.WriteLine($"public abstract class {baseName}");
            writer.WriteLine("{");
            DefineVisitor(writer, baseName, types);

            foreach (string type in types)
            {
                string className = type.Split(':')[0].Trim();
                string fields = type.Split(':')[1].Trim();
                DefineType(writer, baseName, className, fields);
            }

            writer.WriteLine("}");
        }

        private static void DefineType(
            StreamWriter writer,
            string baseName,
            string className,
            string fieldList)
        {
            writer.WriteLine($"    public sealed class {className} : {baseName}");
            writer.WriteLine("    {");

            // Constructor
            writer.WriteLine($"        public {className}({fieldList})");
            writer.WriteLine("        {");

            string[] fields = fieldList.Split(", ");
            foreach (string field in fields)
            {
                string name = field.Split(' ')[1];
                writer.WriteLine($"           this.{name} = {name};");
            }

            writer.WriteLine("        }");
            writer.WriteLine();

            // Fields
            foreach (string field in fields)
            {
                writer.WriteLine($"        public readonly {field};");
            }
            
            writer.WriteLine();
            writer.WriteLine($"        public override R Accept<R>(IVisitor<R> visitor)");
            writer.WriteLine("        {");
            writer.WriteLine(
                $"            return visitor.Visit{className}{baseName}(this);");
            writer.WriteLine("        }");

            writer.WriteLine("    }");
        }
        
        private static void DefineVisitor(
            StreamWriter writer,
            string baseName,
            List<string> types)
        {
            writer.WriteLine($"    public interface IVisitor<R>");
            writer.WriteLine("    {");

            foreach (string type in types)
            {
                string typeName = type.Split(':')[0].Trim();
                writer.WriteLine(
                    $"        R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
            }

            writer.WriteLine("    }");
            writer.WriteLine();
            writer.WriteLine($"    public abstract R Accept<R>(IVisitor<R> visitor);");
        }

    }
}
