﻿using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;
using Microsoft.CSharp;
using Terminal.Gui;

namespace TerminalGuiDesigner
{
    /// <summary>
    /// Converts a <see cref="View"/> in memory into code in a '.Designer.cs' class file
    /// </summary>
    public class ViewToCode
    {
        /// <summary>
        /// Creates a new class file and accompanying '.Designer.cs' file based on
        /// <see cref="Window"/>
        /// </summary>
        /// <param name="csFilePath"></param>
        /// <param name="namespaceName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public View GenerateNewWindow(FileInfo csFilePath, string namespaceName)
        {
            if(csFilePath.Name.EndsWith(CodeToView.ExpectedExtension))
            {
                throw new ArgumentException($@"{nameof(csFilePath)} should be a class file not the designer file e.g. c:\MyProj\MyWindow1.cs");
            }
            string indent = "    ";

            var ns = new CodeNamespace(namespaceName);
            ns.Imports.Add(new CodeNamespaceImport("Terminal.Gui"));

            CodeCompileUnit compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(ns);
            
            var className = Path.GetFileNameWithoutExtension(csFilePath.Name);
            var designerFile = new FileInfo(Path.Combine(csFilePath.Directory.FullName,className + CodeToView.ExpectedExtension));

            CodeTypeDeclaration class1 = new CodeTypeDeclaration(className);
            class1.IsPartial = true;
            class1.BaseTypes.Add(new CodeTypeReference("Window"));

            ns.Types.Add(class1);

            var constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            constructor.Statements.Add(new CodeSnippetStatement($"{indent}{indent}{indent}InitializeComponent();"));

            class1.Members.Add(constructor);

            CSharpCodeProvider provider = new CSharpCodeProvider();

            using (var sw = new StringWriter())
            {
                IndentedTextWriter tw = new IndentedTextWriter(sw, indent);

                // Generate source code using the code provider.
                provider.GenerateCodeFromCompileUnit(compileUnit, tw,
                    new CodeGeneratorOptions());

                tw.Close();

                File.WriteAllText(csFilePath.FullName, sw.ToString());
            }

            var w = new Window();
            w.Add(new Label("Hello World"));

            GenerateDesignerCs(w, designerFile, namespaceName);
            return w;
        }

        private string GenerateDesignerCs(View forView, FileInfo designerFile, string namespaceName)
        {
            var samples = new CodeNamespace(namespaceName);
            samples.Imports.Add(new CodeNamespaceImport("System"));
            samples.Imports.Add(new CodeNamespaceImport("Terminal.Gui"));

            CodeCompileUnit compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(samples);

            CodeTypeDeclaration class1 = new CodeTypeDeclaration("Class1");
            class1.IsPartial = true;

            samples.Types.Add(class1);

            CSharpCodeProvider provider = new CSharpCodeProvider();

            using (var sw = new StringWriter())
            {
                IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");

                // Generate source code using the code provider.
                provider.GenerateCodeFromCompileUnit(compileUnit, tw,
                    new CodeGeneratorOptions());

                tw.Close();


                return sw.ToString();
            }
        }
    }
}
