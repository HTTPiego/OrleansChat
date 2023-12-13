export interface ChatListItem {
    chatId: string
    chatName: string
    isGroup: boolean
    lastSenderName: string
    lastSenderUsername: string
    lastMessage: string
    lastMessageTime: string
    pendingMessages: number
}

export class Message {

}

export class ChatRoom {

}

export class UserPersonalData {
    name: string
    username: string
}

export class UserCompleteData {
    name: string
    username: string
    chats: string[]
    friends: string[]
}

export class FriendCard {
    name: string
    username: string
    befriended: boolean
}
