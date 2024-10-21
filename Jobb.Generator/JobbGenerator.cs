using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using VSLangProj80;
using System.Text;

namespace Jobb.Generator
{
    [ComVisible(true)]
    [Guid("52B316AA-1997-4c81-9969-83604C09EEB4")]
    [CustomToolAttribute("JobbGenerator")]
    public class JobbGenerator 
        : IVsSingleFileGenerator, IVsGeneratorProgress
    {
        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            try
            {
                pbstrDefaultExtension = ".sql";
                return pbstrDefaultExtension.Length; // VSConstants.S_OK;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                pbstrDefaultExtension = string.Empty;
                return -1; // VSConstants.E_FAIL;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wszInputFilePath">Required. Input path of the file.</param>
        /// <param name="bstrInputFileContents"></param>
        /// <param name="wszDefaultNamespace"></param>
        /// <param name="rgbOutputFileContents"></param>
        /// <param name="pcbOutput"></param>
        /// <param name="pGenerateProgress"></param>
        /// <returns></returns>
        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            string generatedContent = "Hello World from VSIX!";

            // Convert the generated content to a byte array
            byte[] bytes = Encoding.UTF8.GetBytes(generatedContent);

            // Allocate memory for the output file content
            int outputLength = bytes.Length;
            rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
            Marshal.Copy(bytes, 0, rgbOutputFileContents[0], outputLength);

            // Set the size of the generated output
            pcbOutput = (uint)outputLength;

            // Use the IVsGeneratorProgress to indicate progress if needed
            pGenerateProgress?.Progress(100, 100); // Indicate completion

            return 0; // VSConstants.S_OK;

            //pcbOutput = 0;
            //return 0;
        }

        public int GeneratorError(int fWarning, uint dwLevel, string bstrError, uint dwLine, uint dwColumn)
        {
            return 0;
        }

        public int Progress(uint nComplete, uint nTotal)
        {
            return 0;
        }
    }
}
