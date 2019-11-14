using System;
using System.IO;
using System.Management.Automation;
using System.Collections.Generic;
using System.Text;

namespace PSTextUtils {


    [Cmdlet(VerbsData.ConvertFrom,"Byte")]
    [OutputType(typeof(String))]

    public class ConvertFromByteCommand : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public byte[] InputObject { get; set; }
        [Parameter]
        [ArgumentCompleter(typeof(EncodingCompleter))]
        [EncodingTransform]
        public System.Text.Encoding Encoding { get; set; } = EncodingTransformAttribute.StandardEncoding["UTF8"];


        private readonly List<byte> bytes = new List<byte>();
        protected override void ProcessRecord()
        {

            bytes.AddRange(InputObject);

        }

        protected override void EndProcessing()
        {
            
            WriteObject(Encoding.GetString(bytes.ToArray()));
            
        }

    }

}