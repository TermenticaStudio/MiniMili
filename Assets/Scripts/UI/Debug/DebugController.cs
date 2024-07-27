using Feature.Jetpack;
using Feature.Player.Movement;
using Logic.Player;
using Logic.Player.WeaponsSystem;
using System;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameDebug.Debug
{
    public class DebugController : MonoBehaviour
    {
        [SerializeField] private WeaponDebugNumericField numericFieldPrefab;
        [SerializeField] private WeaponDebugBoolField booleanFieldPrefab;
        [SerializeField] private WeaponDebugVector2Field vector2FieldPrefab;

        [Header("Weapon Debug")]
        [SerializeField] private Image weaponImage;
        [SerializeField] private TextMeshProUGUI weaponName;
        [SerializeField] private Transform weaponFieldsHolder;
        [SerializeField] private Button fillGunBtn;

        [Header("Jetpack Debug")]
        [SerializeField] private Transform jetpackFieldsHolder;

        [Header("Movement Debug")]
        [SerializeField] private Transform movementFieldsHolder;

        private WeaponPreset _currentWeaponPreset;
        private JetpackData _jetpackData;
        private MovementData _movementData;
        private Player _player;
        private PlayerSpawnHandler _playerSpawnHandler;
        private string _weaponOrigCopy;
        private string _jetpackOrigCopy;
        private string _movementOrigCopy;

        private void Start()
        {
            PlayerSpawnHandler.Instance.OnSpawnPlayer += OnSpawnPlayer;
        }

        private void OnDisable()
        {
            PlayerSpawnHandler.Instance.OnSpawnPlayer -= OnSpawnPlayer;
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(_weaponOrigCopy))
                JsonUtility.FromJsonOverwrite(_weaponOrigCopy, _currentWeaponPreset);
#endif

#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(_jetpackOrigCopy))
                JsonUtility.FromJsonOverwrite(_jetpackOrigCopy, _jetpackData);
#endif

#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(_movementOrigCopy))
                JsonUtility.FromJsonOverwrite(_movementOrigCopy, _movementData);
#endif

            if (_playerSpawnHandler)
                _playerSpawnHandler.OnSpawnPlayer -= OnSpawnPlayer;

            if (_player)
                _player.WeaponsManager.OnChangeWeapon -= OnChangeWeapon;
        }

        private void OnSpawnPlayer(Logic.Player.PlayerInfo obj)
        {
            if (!obj.IsLocal)
                return;

            _player = obj.GetComponent<Player>();
            if (!_player)
                return;

            _player.WeaponsManager.OnChangeWeapon += OnChangeWeapon;
            fillGunBtn.onClick.RemoveAllListeners();
            fillGunBtn.onClick.AddListener(_player.WeaponsManager.RefillWeapon);

            _jetpackData = _player.Jetpack.Data;
            _movementData = _player.Movement.Data;

            InitializeJetpackFields();
            InitializeMovementFields();
        }

        private void OnChangeWeapon(Weapon obj)
        {
            if (obj.Preset == _currentWeaponPreset)
                return;

#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(_weaponOrigCopy))
                JsonUtility.FromJsonOverwrite(_weaponOrigCopy, _currentWeaponPreset);
#endif

            _currentWeaponPreset = obj.Preset;
            _weaponOrigCopy = JsonUtility.ToJson(obj.Preset);
            weaponImage.sprite = _currentWeaponPreset.Icon;
            weaponName.text = _currentWeaponPreset.Name;
            InitializeWeaponFields();
        }

        private void InitializeWeaponFields()
        {
            foreach (Transform field in weaponFieldsHolder)
                Destroy(field.gameObject);

            var members = typeof(WeaponPreset).GetMembers();

            foreach (var member in members)
            {
                if (member.MemberType != MemberTypes.Field)
                    continue;

                var fieldName = member.Name;
                var fieldValue = _currentWeaponPreset.GetType().GetField(member.Name).GetValue(_currentWeaponPreset);

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
                    AddNumericField(displayFieldName.ToString(), floatVal, weaponFieldsHolder, newVal =>
                    {
                        _currentWeaponPreset.GetType().GetField(member.Name).SetValue(_currentWeaponPreset, newVal);
                        _player.WeaponsManager.RefreshWeapon();
                    });
                }

                if (fieldValue.GetType() == typeof(int))
                {
                    var intVal = Convert.ToInt32(fieldValue);
                    AddNumericField(displayFieldName.ToString(), intVal, weaponFieldsHolder, newVal =>
                    {
                        _currentWeaponPreset.GetType().GetField(member.Name).SetValue(_currentWeaponPreset, Convert.ToInt32(newVal));
                        _player.WeaponsManager.RefreshWeapon();
                    });
                }

                if (fieldValue.GetType() == typeof(bool))
                {
                    var boolVal = Convert.ToBoolean(fieldValue);
                    AddBoolField(displayFieldName.ToString(), boolVal, weaponFieldsHolder, newVal =>
                    {
                        _currentWeaponPreset.GetType().GetField(member.Name).SetValue(_currentWeaponPreset, newVal);
                        _player.WeaponsManager.RefreshWeapon();
                    });
                }

                if ((fieldValue.GetType() == typeof(Vector2)))
                {
                    var vector2Val = (Vector2)fieldValue;
                    AddVector2Field(displayFieldName.ToString(), vector2Val, weaponFieldsHolder, newVal =>
                    {
                        _currentWeaponPreset.GetType().GetField(member.Name).SetValue(_currentWeaponPreset, newVal);
                        _player.WeaponsManager.RefreshWeapon();
                    });
                }
            }
        }

        private void InitializeJetpackFields()
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(_jetpackOrigCopy))
                JsonUtility.FromJsonOverwrite(_jetpackOrigCopy, _jetpackData);
#endif

            foreach (Transform field in jetpackFieldsHolder)
                Destroy(field.gameObject);

            var members = typeof(JetpackData).GetMembers();

            foreach (var member in members)
            {
                if (member.MemberType != MemberTypes.Field)
                    continue;

                var fieldName = member.Name;
                var fieldValue = _jetpackData.GetType().GetField(member.Name).GetValue(_jetpackData);

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
                    AddNumericField(displayFieldName.ToString(), floatVal, jetpackFieldsHolder, newVal =>
                    {
                        _jetpackData.GetType().GetField(member.Name).SetValue(_jetpackData, newVal);
                    });
                }

                if (fieldValue.GetType() == typeof(int))
                {
                    var intVal = Convert.ToInt32(fieldValue);
                    AddNumericField(displayFieldName.ToString(), intVal, jetpackFieldsHolder, newVal =>
                    {
                        _jetpackData.GetType().GetField(member.Name).SetValue(_jetpackData, Convert.ToInt32(newVal));
                    });
                }

                if (fieldValue.GetType() == typeof(bool))
                {
                    var boolVal = Convert.ToBoolean(fieldValue);
                    AddBoolField(displayFieldName.ToString(), boolVal, jetpackFieldsHolder, newVal =>
                    {
                        _jetpackData.GetType().GetField(member.Name).SetValue(_jetpackData, newVal);
                    });
                }

                if ((fieldValue.GetType() == typeof(Vector2)))
                {
                    var vector2Val = (Vector2)fieldValue;
                    AddVector2Field(displayFieldName.ToString(), vector2Val, jetpackFieldsHolder, newVal =>
                    {
                        _jetpackData.GetType().GetField(member.Name).SetValue(_jetpackData, newVal);
                    });
                }
            }
        }

        private void InitializeMovementFields()
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(_movementOrigCopy))
                JsonUtility.FromJsonOverwrite(_movementOrigCopy, _movementData);
#endif

            foreach (Transform field in movementFieldsHolder)
                Destroy(field.gameObject);

            var members = typeof(MovementData).GetMembers();

            foreach (var member in members)
            {
                if (member.MemberType != MemberTypes.Field)
                    continue;

                var fieldName = member.Name;
                var fieldValue = _movementData.GetType().GetField(member.Name).GetValue(_movementData);

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
                    AddNumericField(displayFieldName.ToString(), floatVal, movementFieldsHolder, newVal =>
                    {
                        _movementData.GetType().GetField(member.Name).SetValue(_movementData, newVal);
                    });
                }

                if (fieldValue.GetType() == typeof(int))
                {
                    var intVal = Convert.ToInt32(fieldValue);
                    AddNumericField(displayFieldName.ToString(), intVal, movementFieldsHolder, newVal =>
                    {
                        _movementData.GetType().GetField(member.Name).SetValue(_movementData, Convert.ToInt32(newVal));
                    });
                }

                if (fieldValue.GetType() == typeof(bool))
                {
                    var boolVal = Convert.ToBoolean(fieldValue);
                    AddBoolField(displayFieldName.ToString(), boolVal, movementFieldsHolder, newVal =>
                    {
                        _movementData.GetType().GetField(member.Name).SetValue(_movementData, newVal);
                    });
                }

                if ((fieldValue.GetType() == typeof(Vector2)))
                {
                    var vector2Val = (Vector2)fieldValue;
                    AddVector2Field(displayFieldName.ToString(), vector2Val, movementFieldsHolder, newVal =>
                    {
                        _movementData.GetType().GetField(member.Name).SetValue(_movementData, newVal);
                    });
                }
            }
        }

        private void AddNumericField(string title, float field, Transform holder, Action<float> onChange)
        {
            var instance = Instantiate(numericFieldPrefab, holder);
            instance.Setup(title, field, onChange);
        }

        private void AddBoolField(string title, bool value, Transform holder, Action<bool> onChange)
        {
            var instance = Instantiate(booleanFieldPrefab, holder);
            instance.Setup(title, value, onChange);
        }

        private void AddVector2Field(string title, Vector2 field, Transform holder, Action<Vector2> onChange)
        {
            var instance = Instantiate(vector2FieldPrefab, holder);
            instance.Setup(title, field, onChange);
        }
    }
}