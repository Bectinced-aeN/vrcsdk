using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using VRC.Core;

namespace VRC
{
    public class Enter2faCodeEditorWindow : EditorWindow 
    {
        static string authenticationCode = "";

        string usernameOrEmail;
        string password;

        System.Action onAuthenticationVerifiedAction;

        private const string TWO_FACTOR_AUTHENTICATION_HELP_URL = "https://docs.vrchat.com/docs/setup-2fa";

        private const string ENTER_2FA_CODE_TITLE_STRING = "Enter a numeric code from your authenticator app (or one of your saved recovery codes).";
        private const string ENTER_2FA_CODE_LABEL_STRING = "Code:";

        private const string CHECKING_2FA_CODE_STRING = "Checking code...";
        private const string ENTER_2FA_CODE_INVALID_CODE_STRING = "Invalid Code";

        private const string ENTER_2FA_CODE_VERIFY_STRING = "Verify";
        private const string ENTER_2FA_CODE_CANCEL_STRING = "Cancel";
        private const string ENTER_2FA_CODE_HELP_STRING = "Help";

        private const int WARNING_ICON_SIZE = 60;
        private const int WARNING_FONT_HEIGHT = 18;

        bool entered2faCodeIsInvalid;
        bool authorizationCodeWasVerified;

        private int previousAuthenticationCodeLength = 0;

        bool checkingCode;

        private Texture2D warningIconGraphic = null;

        bool IsValidAuthenticationCodeFormat()
        {
            bool isValid2faAuthenticationCode = false;

            if (!string.IsNullOrEmpty(authenticationCode))
            {
                // check if the input is a valid 6-digit numberic code (ignoring spaces)
                Regex rx = new Regex(@"^(\s*\d\s*){6}$", RegexOptions.Compiled);
                MatchCollection matches6DigitCode = rx.Matches(authenticationCode);
                isValid2faAuthenticationCode = (matches6DigitCode.Count == 1);
            }

            return isValid2faAuthenticationCode;
        }

        bool IsValidRecoveryCodeFormat()
        {
            bool isValid2faRecoveryCode = false;

            if (!string.IsNullOrEmpty(authenticationCode))
            {
                // check if the input is a valid 8-digit alpha-numberic code (format xxxx-xxxx) "-" is optional & ignore any spaces
                // OTP codes also exclude the letters i,l,o and the digit 1 to prevent any confusion
                Regex rx = new Regex(@"^(\s*[a-hj-km-np-zA-HJ-KM-NP-Z02-9]\s*){4}-?(\s*[a-hj-km-np-zA-HJ-KM-NP-Z02-9]\s*){4}$", RegexOptions.Compiled);
                MatchCollection matchesRecoveryCode = rx.Matches(authenticationCode);
                isValid2faRecoveryCode = (matchesRecoveryCode.Count == 1);
            }

            return isValid2faRecoveryCode;
        }

        public void SetCredentialsAndVerificationHandling(string username, string password, System.Action onVerifiedAction = null)
        {
            this.usernameOrEmail = username;
            this.password = password;
            onAuthenticationVerifiedAction = onVerifiedAction;
        }

        private void OnEnable()
        {
            entered2faCodeIsInvalid = false;
            warningIconGraphic = Resources.Load("2FAIcons/SDK_Warning_Triangle_icon") as Texture2D;
        }

        void OnGUI()
        {
            const int ENTER_2FA_CODE_BORDER_SIZE = 20;

            const int ENTER_2FA_CODE_BUTTON_WIDTH = 260;
            const int ENTER_2FA_CODE_BUTTON_HEIGHT = 40;

            const int ENTER_2FA_CODE_VERIFY_BUTTON_WIDTH = ENTER_2FA_CODE_BUTTON_WIDTH/2;

            const int ENTER_2FA_CODE_ENTRY_REGION_HEIGHT = 50;
            const int ENTER_2FA_CODE_ENTRY_REGION_WIDTH = 130;

            const int ENTER_2FA_CODE_MIN_WINDOW_WIDTH = ENTER_2FA_CODE_VERIFY_BUTTON_WIDTH + ENTER_2FA_CODE_ENTRY_REGION_WIDTH + (ENTER_2FA_CODE_BORDER_SIZE * 3);
            const int ENTER_2FA_CODE_MIN_WINDOW_HEIGHT = (ENTER_2FA_CODE_BUTTON_HEIGHT * 2) + ENTER_2FA_CODE_ENTRY_REGION_HEIGHT + WARNING_ICON_SIZE + (ENTER_2FA_CODE_BORDER_SIZE * 3);

            this.minSize = new Vector2(ENTER_2FA_CODE_MIN_WINDOW_WIDTH, ENTER_2FA_CODE_MIN_WINDOW_HEIGHT);

            bool isValidAuthenticationCode = IsValidAuthenticationCodeFormat();
            bool isValidRecoveryCode = IsValidRecoveryCodeFormat();

            GUILayout.Space(ENTER_2FA_CODE_BORDER_SIZE);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(ENTER_2FA_CODE_BORDER_SIZE);
            GUILayout.FlexibleSpace();
            GUIStyle titleStyle = new GUIStyle(EditorStyles.label);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.wordWrap = true;
            EditorGUILayout.LabelField(ENTER_2FA_CODE_TITLE_STRING, titleStyle, GUILayout.Width(ENTER_2FA_CODE_MIN_WINDOW_WIDTH - (2 * ENTER_2FA_CODE_BORDER_SIZE)), GUILayout.Height(ENTER_2FA_CODE_BORDER_SIZE * 2), GUILayout.ExpandHeight(true));
            GUILayout.FlexibleSpace();
            GUILayout.Space(ENTER_2FA_CODE_BORDER_SIZE);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(ENTER_2FA_CODE_BORDER_SIZE);
            GUILayout.FlexibleSpace();
            Vector2 size = EditorStyles.boldLabel.CalcSize(new GUIContent(ENTER_2FA_CODE_LABEL_STRING));
            EditorGUILayout.LabelField(ENTER_2FA_CODE_LABEL_STRING, EditorStyles.boldLabel, GUILayout.MaxWidth(size.x));
            authenticationCode = EditorGUILayout.TextField(authenticationCode);

            EditorGUILayout.Space();
            // Verify 2FA code button
            if (GUILayout.Button(ENTER_2FA_CODE_VERIFY_STRING, GUILayout.Width(ENTER_2FA_CODE_VERIFY_BUTTON_WIDTH)))
            {
                checkingCode = true;
                APIUser.VerifyTwoFactorAuthCode(authenticationCode, isValidAuthenticationCode ? API2FA.TIME_BASED_ONE_TIME_PASSWORD_AUTHENTICATION : API2FA.ONE_TIME_PASSWORD_AUTHENTICATION, usernameOrEmail, password,
                        delegate
                        {
                            // valid code
                            entered2faCodeIsInvalid = false;
                            authorizationCodeWasVerified = true;
                            checkingCode = false;
                            if (null != onAuthenticationVerifiedAction)
                                onAuthenticationVerifiedAction();
                            Close();
                        },
                        delegate
                        {
                            entered2faCodeIsInvalid = true;
                            checkingCode = false;
                            Repaint();    // force refresh of GUI to display invalid code message
                        }
                    );
            }

            GUILayout.FlexibleSpace();
            GUILayout.Space(ENTER_2FA_CODE_BORDER_SIZE);
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // after user has entered an invalid code causing the invalid code message to be displayed,
            // edit the code will change it's length meaning it is invalid format, so we can clear the invalid code setting until they resubmit
            if (previousAuthenticationCodeLength != authenticationCode.Length)
            {
                previousAuthenticationCodeLength = authenticationCode.Length;
                entered2faCodeIsInvalid = false;
            }

            // Invalid code text
            if (entered2faCodeIsInvalid)
            {
                GUIStyle s = new GUIStyle(EditorStyles.label);
                s.alignment = TextAnchor.UpperLeft;
                s.normal.textColor = Color.red;
                s.fontSize = WARNING_FONT_HEIGHT;
                s.padding = new RectOffset(0, 0, (WARNING_ICON_SIZE - s.fontSize) / 2, 0);
                s.fixedHeight = WARNING_ICON_SIZE;

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                var textDimensions = s.CalcSize(new GUIContent(ENTER_2FA_CODE_INVALID_CODE_STRING));
                GUILayout.Label(new GUIContent(warningIconGraphic), GUILayout.Width(WARNING_ICON_SIZE), GUILayout.Height(WARNING_ICON_SIZE));
                EditorGUILayout.LabelField(ENTER_2FA_CODE_INVALID_CODE_STRING, s, GUILayout.Width(textDimensions.x));
                EditorGUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            else if (checkingCode)
            {
                // Display checking code message
                EditorGUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUIStyle s = new GUIStyle(EditorStyles.label);
                s.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField(CHECKING_2FA_CODE_STRING, s);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }

            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.Space(ENTER_2FA_CODE_BORDER_SIZE);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.FlexibleSpace();

            // Help button
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(ENTER_2FA_CODE_BORDER_SIZE);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(ENTER_2FA_CODE_HELP_STRING, GUILayout.Width(ENTER_2FA_CODE_BUTTON_WIDTH)))
            {
                Application.OpenURL(TWO_FACTOR_AUTHENTICATION_HELP_URL);
            }
            GUILayout.Space(ENTER_2FA_CODE_BORDER_SIZE);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Cancel button
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(ENTER_2FA_CODE_BORDER_SIZE);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(ENTER_2FA_CODE_CANCEL_STRING, GUILayout.Width(ENTER_2FA_CODE_BUTTON_WIDTH)))
            {
                Close();
            }
            GUILayout.Space(ENTER_2FA_CODE_BORDER_SIZE);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(ENTER_2FA_CODE_BORDER_SIZE);
        }

        private void OnDestroy()
        {
            if (!authorizationCodeWasVerified)
            {
                AccountEditorWindow.Logout();
            }
        }

    }
}
