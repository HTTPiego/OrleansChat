
namespace GrainInterfaces
{
    public interface IGroupChatRoom : IDirectChatRoom
    {

        //Task PostMessage(string message, IUser messageAuthor);

        //Task<List<string>> getMessages();

        Task setGroupCreator(IUser groupCreator);

        Task addAdmin(IUser whoAdds, IUser newAdmin);
                
        Task removeAdmin(IUser whoRemoves, IUser admin);

        //Task addUser(IUser whoAdds, IUser user);

        Task removeUser(IUser whoRemoves, IUser member);

    }
}

//Task Subscribe(IUser user);

//Task Unsubscribe(IUser user);