export interface ChatListItem {
    chatId: string
    isGroup: boolean
    lastMessage: UserMessage
    pendingMessages: number
}

export class UserMessage {
    authorUsername: string
    textMessage: string
    timestamp: Date
    chatRoomName: string
}

export class ChatRoom {
    chatName: string;
    messages: UserMessage[]
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
