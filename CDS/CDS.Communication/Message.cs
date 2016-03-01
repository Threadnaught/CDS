using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Communication
{
    public class Message
    {
        public Operation Op;
        public Guid SenderID;
    }

    public enum Operation : byte
    {
        read = 0, //send emtpy, response data
        write = 1, //send data, response emtpy
        create = 2, //send type, response emtpy
        delete = 3, //send empty, response emtpy
        getType = 4, //send empty, response series of null terminated node names
        getChildren = 5, //send emtpy, response type
        nodeExists = 6, //send empty, response 1 for exists, 0 for doesn't exist
        subscribe = 7, //send empty, response empty
        unsubscribe = 8, //send empty, response empty

        subscriberUpdate = 253,
        requestSuccessful = 254,
        requestFailed = 255,
    }
}
