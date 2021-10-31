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
        
        userJoined.Register(UserJoined);
        userLeft.Register(UserLeft);

        users = new Dictionary<byte, User>();
    }

    private void UserJoined(byte id)
    {
        User user = Instantiate(userPreFab, transform).GetComponent<User>();
        user.id = id;

        users[id] = user;
    }
    
    private void UserLeft(byte id)
    {
        Destroy(users[id]);
        users.Remove(id);
    }
}
