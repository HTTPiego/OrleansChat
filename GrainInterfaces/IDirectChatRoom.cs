namespace GrainInterfaces
{
    public interface IDirectChatRoom : IGrainWithGuidKey
    {
        Task PostMessage(string message, IUser messageAuthor);

        Task<List<string>> getMessages();

        Task addUser(IUser whoAdds, IUser user);

        //Task removeUser(IUser whoRemoves, IUser user);

    }
}

//Task Subscribe(IUser user);

//Task Unsubscribe(IUser user);