//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: testLogin.proto
namespace CommonPublic
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"testLogin")]
  public partial class testLogin : global::ProtoBuf.IExtensible
  {
    public testLogin() {}
    
    private string _LoginName;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"LoginName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string LoginName
    {
      get { return _LoginName; }
      set { _LoginName = value; }
    }
    private string _PassWord = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"PassWord", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string PassWord
    {
      get { return _PassWord; }
      set { _PassWord = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}