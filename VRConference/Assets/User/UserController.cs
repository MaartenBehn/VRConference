using System.Collections.Generic;
using Engine.User;
using UnityEngine;
using Utility;

public class UserController : MonoBehaviour
{
    public static UserController instance;
    
    public Dictionary<byte, User> users;

    [SerializeField] private GameObject userPreFab;
    [SerializeField] private PublicEventByte userJoined;
    [SerializeField] private PublicEventByte userLeft;
    [SerializeField] private PublicEvent unloadEvent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        users = new Dictionary<byte, User>();

        unloadEvent.Register(() =>
        {
            byte[] keys = new byte[users.Keys.Count]; 
            users.Keys.CopyTo(keys, 0);
            
            foreach (byte id in keys)
            {
                UserLeft(id);
            }
        });
    }

    public void UserJoined(byte id)
    {
        User user = Instantiate(userPreFab, transform).GetComponent<User>();
        user.id = id;

        users[id] = user;
        
        userJoined.Raise(id);
    }
    
    public void UserLeft(byte id)
    {
        userLeft.Raise(id);
        
        Destroy(users[id].gameObject);
        users.Remove(id);
    }
}
