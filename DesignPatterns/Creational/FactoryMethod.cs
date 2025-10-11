namespace Creational;
public class FactoryMethod
{
    public static void Main()
    {
        var docFactory = new DocumentFactory([new PdfDocument(), new WordDocument(), new ExcelDocument()]);

        var wordDoc = docFactory.CreateDocument(DocumentType.Word);

        wordDoc.Open();
        wordDoc.Save();
        wordDoc.Close();
    }

    interface IDocument
    {
        public DocumentType DocumentType { get; }
        void Open();
        void Close();
        void Save();
    }

    enum DocumentType
    {
        Pdf,
        Word,
        Excel
    }

    class PdfDocument : IDocument
    {
        public DocumentType DocumentType => DocumentType.Pdf;
        public void Open() { /* Open PDF document */ }
        public void Close() { /* Close PDF document */ }
        public void Save() { /* Save PDF document */ }
    }

    class WordDocument : IDocument
    {
        public DocumentType DocumentType => DocumentType.Word;
        public void Open() { /* Open Word document */ }
        public void Close() { /* Close Word document */ }
        public void Save() { /* Save Word document */ }
    }

    class ExcelDocument : IDocument
    {
        public DocumentType DocumentType => DocumentType.Excel;
        public void Open() { /* Open Excel document */ }
        public void Close() { /* Close Excel document */ }
        public void Save() { /* Save Excel document */ }
    }

    interface IDocumentFactory
    {
        IDocument CreateDocument(DocumentType docType);
    }

    class DocumentFactory : IDocumentFactory
    {
        private Dictionary<DocumentType, IDocument> _docs;
        public DocumentFactory(IEnumerable<IDocument> documents)
        {
            _docs = documents.ToDictionary(d => d.DocumentType, d => d); // Or documents.ToDictionary(d => d.DocumentType);
        }
        public IDocument CreateDocument(DocumentType docType)
        {
            if (_docs.TryGetValue(docType, out IDocument doc))
            {
                return doc;
            }

            throw new NotSupportedException();
        }
    }

    interface ISimpleDocumentFactory
    {
        IDocument CreateDocument();
    }

    class PdfDocumentFactory : ISimpleDocumentFactory
    {
        public IDocument CreateDocument() => new PdfDocument();
    }

    class WordDocumentFactory : ISimpleDocumentFactory
    {
        public IDocument CreateDocument() => new WordDocument();
    }

    class ExcelDocumentFactory : ISimpleDocumentFactory
    {
        public IDocument CreateDocument() => new ExcelDocument();
    }


}
