using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentScanner.Models
{
    public class DocumentFile
    {
        public string FullFilePath { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public int SlNo { get; set; }
        
    }
}
