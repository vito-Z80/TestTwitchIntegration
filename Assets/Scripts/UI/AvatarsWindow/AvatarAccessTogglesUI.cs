using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarsWindow
{
    public class AvatarAccessTogglesUI : MonoBehaviour
    {
        [SerializeField] Toggle moderatorToggle;
        [SerializeField] Toggle partnerToggle;
        [SerializeField] Toggle premiumToggle;
        [SerializeField] Toggle adminToggle;
        [SerializeField] Toggle stuffToggle;
        [SerializeField] Toggle vipToggle;
        [SerializeField] Toggle subscriberToggle;


        public void SetToggleValues(AvatarData avatarData)
        {
            var userData = Utils.DisassembleAccessValue(avatarData.Access);
            moderatorToggle.isOn = userData.IsModerator;
            partnerToggle.isOn = userData.IsPartner;
            premiumToggle.isOn = userData.IsPremium;
            adminToggle.isOn = userData.IsAdmin;
            stuffToggle.isOn = userData.IsStuff;
            vipToggle.isOn = userData.IsVip;
            subscriberToggle.isOn = userData.IsSubscriber;
        }

        public ChatUserData GetUserData()
        {
            var userData = new ChatUserData
            {
                IsModerator = moderatorToggle.isOn,
                IsPartner = partnerToggle.isOn,
                IsPremium = premiumToggle.isOn,
                IsAdmin = adminToggle.isOn,
                IsStuff = stuffToggle.isOn,
                IsVip = vipToggle.isOn,
                IsSubscriber = subscriberToggle.isOn,
            };
            return userData;
        }
    }
}