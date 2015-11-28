namespace CDS.Common 
{
    public enum CDSOperations : byte
    {
        read = 0, //send emtpy, response data
        write = 1, //send data, response emtpy
        create = 2, //send type, response emtpy
        delete = 3, //send empty, response emtpy
        getType = 4, //send empty, response series of null terminated node names
        getChildren = 5, //send emtpy, response type
        subscribe = 6,
        unsubscribe = 7
    }
    public enum CDSResponses : byte
    {
        Success = 0,
        Failure = 1,
        SubscribedUpdate = 2
    }
    public enum ChannelOp : byte
    {
        Create = 0,
        Delete = 1,
        Message = 2,
        ChannelOperationFailed = 3,
    }
    public enum NodeType : byte
    {
        Hollow = 0,
        Data = 1,
        Stream = 2,
    }
    public enum DataType : byte
    {
        Data = 0,
        Link = 1,
    }
}
