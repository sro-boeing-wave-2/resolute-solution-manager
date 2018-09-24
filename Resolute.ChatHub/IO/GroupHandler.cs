using System;
using System.Collections.Generic;


namespace Resolute.ChatHub.IO
{
    public static class GroupHandler
    {
        public static Dictionary<String, List<String>> GroupUsersMapper = new Dictionary<string, List<string>>();

        public static Dictionary<String, String> UserGroupMapper = new Dictionary<string, string>();

        // public static void MapUserToGroup(string userConnectionId, string groupId) {
        //     GroupHandler.UserGroupMapper.Add(userConnectionId, groupId);

        //     if (GroupHandler.GroupUsersMapper.ContainsKey(groupId))
        //     {
        //         var listOfUsersInGroup = GroupHandler.GroupUsersMapper[groupId];
        //         listOfUsersInGroup.Add(userConnectionId);
        //     }
        //     else
        //     {
        //         GroupHandler.GroupUsersMapper.Add(groupId, new List<String>() { userConnectionId });
        //     }
        // };

        // public static string GetGroup()
    }
}
