using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CommonPublic
{
    public class ByteHelper
    {
        public static ByteHelper instance;
        static ByteHelper()
        {
            if (instance == null)
            {
                instance = new ByteHelper();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sendmsg"></param>
        /// <param name="cmdCode"></param>
        public void SetCmdCodeToDataByte<T>(byte[] send_datas, T sendmsg, int cmdCode)
        {
            MemoryStream ms = new MemoryStream();
            ProtoBuf.Serializer.SerializeWithLengthPrefix(ms, sendmsg, PrefixStyle.Base128);
            byte[] typeID = BitConverter.GetBytes(cmdCode);
            typeID.CopyTo(send_datas, 0);
            ms.GetBuffer().CopyTo(send_datas, 4);
        }

        public void GetCmdCodeToDataByte(byte[] result, out byte[] outDataBytes, out int cmdCode)
        {
            byte[] dataBytes = new byte[4096];
            byte[] codeBytes = new byte[4];
            System.Buffer.BlockCopy(result, 0, codeBytes, 0, 4);
            System.Buffer.BlockCopy(result, 4, dataBytes, 0, 1020);
            cmdCode = BitConverter.ToInt32(codeBytes, 0);
            outDataBytes = dataBytes;
        }
    }
}
