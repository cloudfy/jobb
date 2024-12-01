using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Jobb.Schemas;
using Jobb.IO;

namespace Jobb.Vsix
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(JobbGenerator.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [InstalledProductRegistration("JobbGenerator", "Generates SQL from a Jobb file", "1.0")]
    [ComVisible(true)]
    [ProvideObject(typeof(JobbGenerator))]
    [JobbGeneratorRegistration(typeof(JobbGenerator), "JobbGenerator", JobbGenerator.CsharpProject, GeneratesDesignTimeSource = true)]
    public sealed class JobbGenerator 
        : IVsSingleFileGenerator
    {
        public const string PackageGuidString = "a95589dd-b696-48aa-b934-f36b799d2db0";
        public const string CsharpProject = "{FAE04EC1-301F-11D3-BF4B-00C04F79EFBC}";

        public JobbGenerator()
        {
        }

        #region === IVsSingleFileGenerator ===
        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".sql";
            return pbstrDefaultExtension.Length;
        }

        private byte[] Generate(string inputFile, string inputContent, IVsGeneratorProgress generatorProgress)
        {
            var jobbFile = IOHelper.ReadContent(inputFile, inputContent);
            
            SchemaGenerator generator = new SchemaGenerator();
            generator.GenerationProgress += (sender, e) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                generatorProgress.Progress(30 + (uint)(e.Step * 5), 100);
            };
            return ThreadHelper.JoinableTaskFactory.Run<byte[]>(async delegate
            {
                return await generator.Generate(jobbFile);
            });
        }

        public int Generate(
            string wszInputFilePath
            , string bstrInputFileContents
            , string wszDefaultNamespace
            , IntPtr[] rgbOutputFileContents
            , out uint pcbOutput
            , IVsGeneratorProgress pGenerateProgress)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (IOHelper.IsJobbFile(wszInputFilePath) == false)
            {
                FileLog.Write("Not a jobb file: " + wszInputFilePath);

                pcbOutput = 0;
                return VSConstants.S_FALSE;
            }

            try
            {
                pGenerateProgress.Progress(20, 100);

                var bytes = Generate(wszInputFilePath, bstrInputFileContents, pGenerateProgress);

                pGenerateProgress.Progress(80, 100);

                int length = bytes.Length;
                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(length);
                Marshal.Copy(bytes, 0, rgbOutputFileContents[0], length);
                pcbOutput = (uint)length;

                pGenerateProgress.Progress(100, 100);
            }
            catch (Exception ex)
            {
                FileLog.WriteError(ex);

                pGenerateProgress.GeneratorError(4, 1, ex.Message, 1, 1);
                pcbOutput = 0;
                return VSConstants.S_FALSE;
            }
            return VSConstants.S_OK;
        }
        #endregion
    }
}
