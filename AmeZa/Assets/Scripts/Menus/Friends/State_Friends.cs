using SeganX;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Friends : GameState
{
    [SerializeField] private UIFriendItem itemPrefab = null;
    [SerializeField] private Button addFriendButton = null;

    private List<Online.Friends.Friendship> friends = new List<Online.Friends.Friendship>();

    private void Start()
    {
        itemPrefab.gameObject.SetActive(false);

        addFriendButton.onClick.AddListener(() => game.OpenPopup<Popup_AddFriend>().Setup(friends.Count < GlobalConfig.Friends.maxCount, friend =>
        {
            if (friends.Exists(x => x.id == friend.id) == false)
            {
                friends.Add(friend);
                SortFriends();
                itemPrefab.Clone<UIFriendItem>().Setup(friend, friends.Count).gameObject.SetActive(true);
            }
        }));

        Loading.Show();
        if (Profile.IsLoggedIn)
        {
            LoadFriends();
        }
        else
        {
            Profile.Sync(false, success =>
            {
                if (success)
                {
                    LoadFriends();
                }
                else
                {
                    Loading.Hide();
                    Back();
                }
            });
        }
    }

    private void LoadFriends()
    {
        Online.Friends.Get((success, list) =>
        {
            Loading.Hide();
            if (success)
            {
                friends.Clear();
                friends.AddRange(list);

                // add me :)
                friends.Add(new Online.Friends.Friendship() { id = "0", avatar = Profile.Avatar.Json, level = Profile.GetLevelsPassed().ToString(), nickname = Profile.Nickname, status = Profile.Status, username = Profile.Username });

                SortFriends();
                for (int i = 0; i < friends.Count; i++)
                    itemPrefab.Clone<UIFriendItem>().Setup(friends[i], i + 1).gameObject.SetActive(true);
                UiShowHide.ShowAll(transform);
            }
            else Back();
        });
    }

    private void SortFriends()
    {
        friends.Sort((x, y) => y.level.ToInt() - x.level.ToInt());
    }
}
