using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UTanksClient;

namespace SecuredSpace.UI.GameUI
{
    public class LoginRegistrationUIHandler : MonoBehaviour
    {
        public GameObject LoginWindow;
        public InputField LoginLoginInput;
        public Text LoginLoginLabel;
        public InputField LoginPasswordInput;
        public Text LoginPasswordLabel;
        public Button LoginEnter;
        public Button GoToRegistration;
        public Toggle LoginAutosave;

        

        [Space(10)]

        public GameObject RegistrationWindow;
        public InputField RegisterLoginInput;
        public Text RegisterLoginLabel;
        public InputField RegisterPasswordInput;
        public Text RegisterPasswordLabel;
        public InputField RegisterConfirmPasswordInput;
        public Text RegisterConfirmPasswordLabel;
        public InputField RegisterEmailInput;
        public Text RegisterEmailLabel;
        public Button RegisterEnter;
        public Button GoToLogin;
        public Toggle RegisterAutosave;



        void Start()
        {
            LoginEnter.GetComponent<Button>().onClick.AddListener(OnLoginAttempt);
            RegisterEnter.GetComponent<Button>().onClick.AddListener(OnRegisterAttempt);
            RegisterLoginInput.GetComponent<InputField>().onValueChanged.AddListener(OnRegisterLoginInputChecker);
            GoToRegistration.GetComponent<Button>().onClick.AddListener(GoToRegistrationWindow);
            GoToLogin.GetComponent<Button>().onClick.AddListener(GoToLoginWindow);
        }

        void OnLoginAttempt()
        {
            ClientNetworkService.instance.AttemptLogin(LoginLoginInput.GetComponent<InputField>().text, LoginPasswordInput.GetComponent<InputField>().text, "", (packet) => {
                MessageBoxProvider.ShowInfo($"Error login or password, or {packet.reason}", "");
            });
        }

        void OnRegisterLoginInputChecker(string newValue)
        {

        }

        void OnRegisterAttempt()
        {
            ClientNetworkService.instance.AttemptRegister(RegisterLoginInput.GetComponent<InputField>().text, RegisterPasswordInput.GetComponent<InputField>().text, RegisterEmailInput.GetComponent<InputField>().text, "", (packet) => {
                MessageBoxProvider.ShowInfo($"Error login or password, or {packet.reason}", "");
            });
        }

        void GoToRegistrationWindow()
        {
            LoginWindow.SetActive(false);
            RegistrationWindow.SetActive(true);
        }

        void GoToLoginWindow()
        {
            LoginWindow.SetActive(true);
            RegistrationWindow.SetActive(false);
        }
    }

}