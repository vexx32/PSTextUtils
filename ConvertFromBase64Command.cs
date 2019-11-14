using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;

namespace PSTextUtils
{
    [Cmdlet(VerbsData.ConvertFrom, "Base64")]
    [OutputType(typeof(string), ParameterSetName = new[] { StandardSet })]
    [OutputType(typeof(byte), ParameterSetName = new[] { ByteSet })]
    public class ConvertFromBase64Command : PSCmdlet
    {
        private const string StandardSet = "StandardSet";
        private const string ByteSet = "ByteOutputSet";
        private const string FileSet = "FileOutputSet";

        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Alias("InputString", "String", "Text")]
        public string InputObject { get; set; }

        [Parameter(ParameterSetName = StandardSet)]
        [ArgumentCompleter(typeof(EncodingCompleter))]
        [EncodingTransform]
        public Encoding Encoding { get; set; } = EncodingTransformAttribute.StandardEncoding["UTF8"];

        [Parameter(Mandatory = true, ParameterSetName = FileSet)]
        [Alias("LiteralPath", "PSPath")]
        public string Path { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ByteSet)]
        public SwitchParameter ToBytes { get; set; }

        protected override void BeginProcessing()
        {
            if (ParameterSetName == FileSet)
            {
                if (WildcardPattern.ContainsWildcardCharacters(Path))
                {
                    var exception = new ArgumentException(
                        "Non-literal paths are not supported by ConvertFrom-Base64.",
                        nameof(Path));
                    ThrowTerminatingError(new ErrorRecord(
                        exception,
                        "PSTextUtils.InvalidPath",
                        ErrorCategory.InvalidArgument,
                        Path));
                }
                else if (!InvokeProvider.Item.Exists(Path, force: false, literalPath: true))
                {
                    InvokeProvider.Item.New(Path, null, "File", null);
                }
            }
        }

        /// <summary>
        /// ProcessRecord() implementation for ConvertFromBase64Command.
        /// </summary>
        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case StandardSet:
                    byte[] bytes = Convert.FromBase64String(InputObject);
                    WriteObject(Encoding.GetString(bytes));
                    return;
                case ByteSet:
                    WriteObject(Convert.FromBase64String(InputObject), true);
                    return;
                case FileSet:
                    fileBytes.AddRange(Convert.FromBase64String(InputObject));
                    return;
            }
        }

        private readonly List<byte> fileBytes = new List<byte>();

        /// <summary>
        /// EndProcessing() implementation for ConvertFromBase64Command.
        /// </summary>
        protected override void EndProcessing()
        {
            if (ParameterSetName == FileSet)
            {
                var path = SessionState.Path.GetResolvedProviderPathFromPSPath(Path, out _)[0];
                File.WriteAllBytes(path, fileBytes.ToArray());
            }
        }
    }
}
