using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.AzureAD.EasyOAuth.Utils
{
    public enum Resources
    {
        MicrosoftGraph,
        AzureGraph
    }
    public enum Scopes
    {
        Calendars_Read,
        Calendars_ReadWrite,
        Contacts_Read,
        Contacts_ReadWrite,
        Directory_AccessAsUser_All,
        Directory_Read_All,
        Directory_ReadWrite_All,
        Files_Read,
        Files_Read_All,
        Files_Read_Selected,
        Files_ReadWrite,
        Files_ReadWrite_All,
        Files_ReadWrite_AppFolder,
        Files_ReadWrite_Selected,
        Group_Read_All,
        Group_ReadWrite_All,
        Mail_Read,
        Mail_ReadWrite,
        Mail_Send,
        Notes_Create,
        Notes_Read,
        Notes_Read_All,
        Notes_ReadWrite,
        Notes_ReadWrite_All,
        Notes_ReadWrite_CreatedByApp,
        offline_access,
        openid,
        People_Read,
        Sites_Read_All,
        Tasks_ReadWrite_All,
        User_Read,
        User_Read_All,
        User_ReadBasic_All,
        User_ReadWrite,
        User_ReadWrite_All,
    };

    public static class AppScopes
    {
        private static Dictionary<string, Scopes> ScopeFromName = new Dictionary<string, Scopes>()
        {
            { "Calendars.Read", Scopes.Calendars_Read },
            { "Calendars.ReadWrite", Scopes.Calendars_ReadWrite },
            { "Contacts.Read", Scopes.Contacts_Read },
            { "Contacts.ReadWrite", Scopes.Contacts_ReadWrite },
            { "Directory.AccessAsUser.All", Scopes.Directory_AccessAsUser_All },
            { "Directory.Read.All", Scopes.Directory_Read_All },
            { "Directory.ReadWrite.All", Scopes.Directory_ReadWrite_All },
            { "Files.Read", Scopes.Files_Read },
            { "Files.Read.All", Scopes.Files_Read_All },
            { "Files.Read.Selected", Scopes.Files_Read_Selected },
            { "Files.ReadWrite", Scopes.Files_ReadWrite },
            { "Files.ReadWrite.All", Scopes.Files_ReadWrite_All },
            { "Files.ReadWrite.AppFolder", Scopes.Files_ReadWrite_AppFolder },
            { "Files.ReadWrite.Selected", Scopes.Files_ReadWrite_Selected },
            { "Group.Read.All", Scopes.Group_Read_All },
            { "Group.ReadWrite.All", Scopes.Group_ReadWrite_All },
            { "Mail.Read", Scopes.Mail_Read },
            { "Mail.ReadWrite", Scopes.Mail_ReadWrite },
            { "Mail.Send", Scopes.Mail_Send },
            { "Notes.Create", Scopes.Notes_Create },
            { "Notes.Read", Scopes.Notes_Read },
            { "Notes.Read.All", Scopes.Notes_Read_All },
            { "Notes.ReadWrite", Scopes.Notes_ReadWrite },
            { "Notes.ReadWrite.All", Scopes.Notes_ReadWrite_All },
            { "Notes.ReadWrite.CreatedByApp", Scopes.Notes_ReadWrite_CreatedByApp },
            { "offline_access", Scopes.offline_access },
            { "openid", Scopes.openid },
            { "People.Read", Scopes.People_Read },
            { "Sites.Read.All", Scopes.Sites_Read_All },
            { "Tasks.ReadWrite.All", Scopes.Tasks_ReadWrite_All },
            { "User.Read", Scopes.User_Read },
            { "User.Read.All", Scopes.User_Read_All },
            { "User.ReadBasic.All", Scopes.User_ReadBasic_All },
            { "User.ReadWrite", Scopes.User_ReadWrite },
            { "User.ReadWrite.All", Scopes.User_ReadWrite_All }
        };

        public static Dictionary<Scopes, string> ScopeNames = new Dictionary<Scopes, string>()
        {
            { Scopes.Calendars_Read, "Read user calendars" },
            { Scopes.Calendars_ReadWrite, "Have full access to user calendars" },
            { Scopes.Contacts_Read, "Read user contacts" },
            { Scopes.Contacts_ReadWrite, "Have full access to user contacts" },
            { Scopes.Directory_AccessAsUser_All, "Access directory as the signed in user" },
            { Scopes.Directory_Read_All, "Read directory data" },
            { Scopes.Directory_ReadWrite_All, "Read and write directory data" },
            { Scopes.Files_Read, "Read user files and files shared with user" },
            { Scopes.Files_Read_All, "Read all files that user can access" },
            { Scopes.Files_Read_Selected, "Read files that the user selects" },
            { Scopes.Files_ReadWrite, "Have full access to user files and files shared with user" },
            { Scopes.Files_ReadWrite_All, "Have full access to all files user can access" },
            { Scopes.Files_ReadWrite_AppFolder, "Have full access to the application's folder" },
            { Scopes.Files_ReadWrite_Selected, "Read and write files that the user selects" },
            { Scopes.Group_Read_All, "Read all groups" },
            { Scopes.Group_ReadWrite_All, "Read and write all groups" },
            { Scopes.Mail_Read, "Read user mail" },
            { Scopes.Mail_ReadWrite, "Read and write access to user mail" },
            { Scopes.Mail_Send, "Send mail as a user" },
            { Scopes.Notes_Create, "Create pages in user notebooks (preview)" },
            { Scopes.Notes_Read, "Read user notebooks (preview)" },
            { Scopes.Notes_Read_All, "Read all notebooks that the user can access (preview)" },
            { Scopes.Notes_ReadWrite, "Read and write user notebooks (preview)" },
            { Scopes.Notes_ReadWrite_All, "Read and write notebooks that the user can access (preview)" },
            { Scopes.Notes_ReadWrite_CreatedByApp, "Limited notebook access (preview)" },
            { Scopes.offline_access, "Access user's data anytime" },
            { Scopes.openid, "Sign users in" },
            { Scopes.People_Read, "Read users' relevant people lists (preview)" },
            { Scopes.Sites_Read_All, "Read items in all site collections" },
            { Scopes.Tasks_ReadWrite_All, "Create, read, update and delete user tasks and projects (preview)" },
            { Scopes.User_Read, "Sign in and read user profile" },
            { Scopes.User_Read_All, "Read all users' full profiles" },
            { Scopes.User_ReadBasic_All, "Read all users' basic profiles" },
            { Scopes.User_ReadWrite, "Read and write access to user profile" },
            { Scopes.User_ReadWrite_All, "Read and write all users' full profiles" }
        };

        public static Dictionary<Scopes, string> ScopeIds = new Dictionary<Scopes, string>()
        {
            { Scopes.Calendars_Read, "465a38f9-76ea-45b9-9f34-9e8b0d4b0b42" },
            { Scopes.Calendars_ReadWrite, "1ec239c2-d7c9-4623-a91a-a9775856bb36" },
            { Scopes.Contacts_Read, "ff74d97f-43af-4b68-9f2a-b77ee6968c5d" },
            { Scopes.Contacts_ReadWrite, "d56682ec-c09e-4743-aaf4-1a3aac4caa21" },
            { Scopes.Directory_AccessAsUser_All, "0e263e50-5827-48a4-b97c-d940288653c7" },
            { Scopes.Directory_Read_All, "06da0dbc-49e2-44d2-8312-53f166ab848a" },
            { Scopes.Directory_ReadWrite_All, "c5366453-9fb0-48a5-a156-24f0c49a4b84" },
            { Scopes.Files_Read, "10465720-29dd-4523-a11a-6a75c743c9d9" },
            { Scopes.Files_Read_All, "df85f4d6-205c-4ac5-a5ea-6bf408dba283" },
            { Scopes.Files_Read_Selected, "5447fe39-cb82-4c1a-b977-520e67e724eb" },
            { Scopes.Files_ReadWrite, "5c28f0bf-8a70-41f1-8ab2-9032436ddb65" },
            { Scopes.Files_ReadWrite_All, "863451e7-0667-486c-a5d6-d135439485f0" },
            { Scopes.Files_ReadWrite_AppFolder, "8019c312-3263-48e6-825e-2b833497195b" },
            { Scopes.Files_ReadWrite_Selected, "17dde5bd-8c17-420f-a486-969730c1b827" },
            { Scopes.Group_Read_All, "5f8c59db-677d-491f-a6b8-5f174b11ec1d" },
            { Scopes.Group_ReadWrite_All, "4e46008b-f24c-477d-8fff-7bb4ec7aafe0" },
            { Scopes.Mail_Read, "570282fd-fa5c-430d-a7fd-fc8dc98a9dca" },
            { Scopes.Mail_ReadWrite, "024d486e-b451-40bb-833d-3e66d98c5c73" },
            { Scopes.Mail_Send, "e383f46e-2787-4529-855e-0e479a3ffac0" },
            { Scopes.Notes_Create, "9d822255-d64d-4b7a-afdb-833b9a97ed02" },
            { Scopes.Notes_Read, "371361e4-b9e2-4a3f-8315-2a301a3b0a3d" },
            { Scopes.Notes_Read_All, "dfabfca6-ee36-4db2-8208-7a28381419b3" },
            { Scopes.Notes_ReadWrite, "615e26af-c38a-4150-ae3e-c3b0d4cb1d6a" },
            { Scopes.Notes_ReadWrite_All, "64ac0503-b4fa-45d9-b544-71a463f05da0" },
            { Scopes.Notes_ReadWrite_CreatedByApp, "ed68249d-017c-4df5-9113-e684c7f8760b" },
            { Scopes.offline_access, "7427e0e9-2fba-42fe-b0c0-848c9e6a8182" },
            { Scopes.openid, "37f7f235-527c-4136-accd-4a02d197296e" },
            { Scopes.People_Read, "ba47897c-39ec-4d83-8086-ee8256fa737d" },
            { Scopes.Sites_Read_All, "205e70e5-aba6-4c52-a976-6d2d46c48043" },
            { Scopes.Tasks_ReadWrite_All, "2219042f-cab5-40cc-b0d2-16b1540b4c5f" },
            { Scopes.User_Read, "e1fe6dd8-ba31-4d61-89e7-88639da4683d" },
            { Scopes.User_Read_All, "a154be20-db9c-4678-8ab7-66f6cc099a59" },
            { Scopes.User_ReadBasic_All, "b340eb25-3456-403f-be2f-af7a0d370277" },
            { Scopes.User_ReadWrite, "b4e74841-8e56-480b-be8b-910348b18b4c" },
            { Scopes.User_ReadWrite_All, "204e0828-b5ca-4ad8-b9f3-f32a958e7cc4" }
        };

        public static Dictionary<Resources, string> ResourceIds = new Dictionary<Resources, string>()
        {
            { Resources.MicrosoftGraph, "00000003-0000-0000-c000-000000000000" },
            { Resources.AzureGraph, "00000002-0000-0000-c000-000000000000" }
        };

        public static Resources GetScopeResource(Scopes scope)
        {
            //HACK: right now only Microsoft Graph Resources Supported
            return Resources.MicrosoftGraph;
        }
    }
}
