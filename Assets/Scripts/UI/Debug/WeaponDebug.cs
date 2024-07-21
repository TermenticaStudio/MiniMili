using Logic.Player;
using Logic.Player.WeaponsSystem;
using System;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameDebug.WeaponDebug
{
    public class WeaponDebug : MonoBehaviour
    {
        [SerializeField] private Image weaponImage;
        [SerializeField] private TextMeshProUGUI weaponName;
        [SerializeField] private Transform fieldsHolder;
        [SerializeField] private WeaponDebugNumericField numericFieldPrefab;
        [SerializeField] private WeaponDebugBoolField booleanFieldPrefab;
        [SerializeField] private WeaponDebugVector2Field vector2FieldPrefab;

        private WeaponPreset currentWeaponPreset;

        private Player player;
        private PlayerSpawnHandler playerSpawnHandler;

        private string origCopy;

        private void Start()
        {
            playerSpawnHandler = FindObjectOfType<PlayerSpawnHandler>();
            playerSpawnHandler.OnSpawnPlayer += OnSpawnPlayer;
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(origCopy))
                JsonUtility.FromJsonOverwrite(origCopy, currentWeaponPreset);
#endif

            if (playerSpawnHandler)
                playerSpawnHandler.OnSpawnPlayer -= OnSpawnPlayer;

            if (player)
                player.WeaponsManager.OnChangeWeapon -= OnChangeWeapon;
        }

        private void OnSpawnPlayer(Logic.Player.PlayerInfo obj)
        {
            if (!obj.IsLocal)
                return;

            player = obj.GetComponent<Player>();
            if (!player)
                return;

            player.WeaponsManager.OnChangeWeapon += OnChangeWeapon;
        }

        private void OnChangeWeapon(Weapon obj)
        {
            if (obj.Preset == currentWeaponPreset)
                return;

#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(origCopy))
                JsonUtility.FromJsonOverwrite(origCopy, currentWeaponPreset);
#endif

            currentWeaponPreset = obj.Preset;
            origCopy = JsonUtility.ToJson(obj.Preset);
            weaponImage.sprite = currentWeaponPreset.Icon;
            weaponName.text = currentWeaponPreset.Name;
            InitializeFields();
        }

        private void InitializeFields()
        {
            foreach (Transform field in fieldsHolder)
                Destroy(field.gameObject);

            var members = typeof(WeaponPreset).GetMembers();

            foreach (var member in members)
            {
                if (member.MemberType != MemberTypes.Field)
                    continue;

                var fieldName = member.Name;
                var fieldValue = currentWeaponPreset.GetType().GetField(member.Name).GetValue(currentWeaponPreset);

                var displayFieldName = new StringBuilder();

                for (int i = 0; i < fieldName.Length; i++)
                {
                    if (char.IsUpper(fieldName[i]))
                        displayFieldName.Append(" ");

                    displayFieldName.Append(fieldName[i]);
                }

                if (fieldValue.GetType() == typeof(float))
                {
                    var floatVal = Convert.ToSingle(fieldValue);
                    AddNumericField(displayFieldName.ToString(), floatVal, newVal =>
                    {
                        currentWeaponPreset.GetType().GetField(member.Name).SetValue(currentWeaponPreset, newVal);
                        player.WeaponsManager.RefreshWeapon();
                    });
                }

                if (fieldValue.GetType() == typeof(int))
                {
                    var intVal = Convert.ToInt32(fieldValue);
                    AddNumericField(displayFieldName.ToString(), intVal, newVal =>
                    {
                        currentWeaponPreset.GetType().GetField(member.Name).SetValue(currentWeaponPreset, Convert.ToInt32(newVal));
                        player.WeaponsManager.RefreshWeapon();
                    });
                }

                if (fieldValue.GetType() == typeof(bool))
                {
                    var boolVal = Convert.ToBoolean(fieldValue);
                    AddBoolField(displayFieldName.ToString(), boolVal, newVal =>
                    {
                        currentWeaponPreset.GetType().GetField(member.Name).SetValue(currentWeaponPreset, newVal);
                        player.WeaponsManager.RefreshWeapon();
                    });
                }

                if ((fieldValue.GetType() == typeof(Vector2)))
                {
                    var vector2Val = (Vector2)fieldValue;
                    AddVector2Field(displayFieldName.ToString(), vector2Val, newVal =>
                    {
                        currentWeaponPreset.GetType().GetField(member.Name).SetValue(currentWeaponPreset, newVal);
                        player.WeaponsManager.RefreshWeapon();
                    });
                }
            }
        }

        private void AddNumericField(string title, float field, Action<float> onChange)
        {
            var instance = Instantiate(numericFieldPrefab, fieldsHolder);
            instance.Setup(title, field, onChange);
        }

        private void AddBoolField(string title, bool value, Action<bool> onChange)
        {
            var instance = Instantiate(booleanFieldPrefab, fieldsHolder);
            instance.Setup(title, value, onChange);
        }

        private void AddVector2Field(string title, Vector2 field, Action<Vector2> onChange)
        {
            var instance = Instantiate(vector2FieldPrefab, fieldsHolder);
            instance.Setup(title, field, onChange);
        }
    }
}