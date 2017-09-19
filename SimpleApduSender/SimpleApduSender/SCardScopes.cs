namespace SimpleApduSender
{
    public enum SCardScopes
    {
        // Scope in user space.
        User,

        // Scope in terminal.
        Terminal,

        // Scope in system. Service on the local machine.
        System,

        // Scope is global.
        Global
    }
}
