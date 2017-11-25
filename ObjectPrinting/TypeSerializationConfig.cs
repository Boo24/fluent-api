namespace ObjectPrinting
{
    public class TypeSerializationConfig<TOwner>
    {
        private PrintingConfig<TOwner> printingConfig;
        public TypeSerializationConfig(PrintingConfig<TOwner> config) => printingConfig = config;
        public TypeConfig<TOwner,TType> ForTarget<TType>() => new TypeConfig<TOwner, TType>(printingConfig);
    }
}
