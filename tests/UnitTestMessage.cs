using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests;

using AnimalCrossing.Shared;

[TestClass]
public class UnitTestMessage
{
    [TestMethod]
    public void TestMessagePull()
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);
        MessagePull pull = new MessagePull();
        pull.Serialize(bw);

        Message? newPull = Message.Parse(stream.ToArray());

        Assert.IsNotNull(newPull);
        Assert.IsTrue(newPull is MessagePull);
        Assert.AreEqual(newPull.Type, pull.Type);
    }

    [TestMethod]
    public void TestMessageState()
    {
        string password = "test";
        DateTime modifiedAt = new DateTime();
        string hash = "hash";
        bool playing = true;
        
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);
        MessageState state = new MessageState(password, modifiedAt, hash, playing);
        state.Serialize(bw);
        
        Message? ns = Message.Parse(stream.ToArray());
        
        Assert.IsNotNull(ns);
        Assert.IsTrue(ns is MessageState);
        MessageState newState = (ns as MessageState)!;
        Assert.AreEqual(state.Password, newState.Password);
        Assert.AreEqual(state.ModifiedAt, newState.ModifiedAt);
        Assert.AreEqual(state.Hash, newState.Hash);
        Assert.AreEqual(state.Playing, newState.Playing);
    }

    [TestMethod]
    public void TestMessagePush()
    {
        string init = "BONJOUR";
        byte[] content = Encoding.ASCII.GetBytes(init);
        DateTime modifiedAt = new DateTime();
        
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);
        MessagePush push = new MessagePush(content, modifiedAt);
        push.Serialize(bw);
        
        Message? ns = Message.Parse(stream.ToArray());
        
        Assert.IsNotNull(ns);
        Assert.IsTrue(ns is MessagePush);
        MessagePush newPush = (ns as MessagePush)!;
        
        Assert.AreEqual(init, Encoding.ASCII.GetString(newPush.Content));
        Assert.AreEqual(push.ModifiedAt, newPush.ModifiedAt);
    }
}