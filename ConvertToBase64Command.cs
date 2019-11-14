using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;

namespace PSTextUtils
{
    [Cmdlet(VerbsData.ConvertTo, "Base64")]
    [OutputType(typeof(string))]
    public class ConvertToBase64Command : PSCmdlet
    {
        private const string ObjectSet = "ObjectParameterSet";
        private const string StringSet = "StringParameterSet";

        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ParameterSetName = ObjectSet)]
        public PSObject InputObject { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = StringSet)]
        [Alias("String", "Text")]
        public string InputString { get; set; }

        [Parameter]
        [ArgumentCompleter(typeof(EncodingCompleter))]
        [EncodingTransform]
        public Encoding Encoding { get; set; } = EncodingTransformAttribute.StandardEncoding["UTF8"];

        /// <summary>
        /// ProcessRecord() implementation for ConvertToBase64Command.
        /// </summary>
        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case ObjectSet:
                    ProcessObject();
                    break;
                case StringSet:
                    ProcessString();
                    break;
            }
        }

        protected override void EndProcessing()
        {
            if (buffer.Count > 0)
            {
                WriteObject(Convert.ToBase64String(buffer.ToArray()));
            }
        }

        private void ProcessString()
        {
            byte[] bytes = Encoding.GetBytes(InputString);
            WriteObject(Convert.ToBase64String(bytes));
        }

        private readonly List<byte> buffer = new List<byte>();

        private void ProcessObject()
        {
            if (InputObject.BaseObject is Stream data)
            {
                while (data.CanRead)
                {
                    buffer.Add((byte)data.ReadByte());
                }

                WriteObject(Convert.ToBase64String(buffer.ToArray()));
                buffer.Clear();
                return;
            }

            if (InputObject.BaseObject is FileInfo fileInfo)
            {
                WriteObject(Convert.ToBase64String(File.ReadAllBytes(fileInfo.FullName)));
            }

            if (LanguagePrimitives.TryConvertTo<byte>(InputObject.BaseObject, out byte byteValue))
            {
                buffer.Add(byteValue);
                return;
            }
        }
    }
}
