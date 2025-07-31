//
//  FormInputField.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.09.23.
//

import SwiftUI
import Combine


struct EIDInputField: View {
    // MARK: - Right icon
    enum RightIconType {
        case none
        case arrowDown
        case arrowRight
        case cross
        case calendar
        
        var iconName: String {
            switch self {
            case .none:
                return ""
            case .arrowDown:
                return "icon_arrow_down_dark"
            case .arrowRight:
                return "icon_arrow_right_dark"
            case .cross:
                return "icon_cross_red"
            case .calendar:
                return "icon_calendar"
            }
        }
    }
    
    // MARK: - Properties
    /// Texts
    @State var title: String = ""
    @State var hint: String = ""
    @Binding var text: String
    /// Error state
    @Binding var showError: Bool
    @Binding var errorText: String
    var shouldValidate: Bool = false
    /// Right button
    @State var rightIcon: RightIconType = .none
    var rightIconAction: () -> Void = {}
    /// Disabled state
    @Environment(\.isEnabled) private var isEnabled: Bool
    /// Picker
    var isPicker: Bool = false
    var tapAction: () -> Void = {}
    /// Flags
    var isMandatory: Bool = false
    var isPassword: Bool = false
    /// Text options
    var textAlignment: TextAlignment = .leading
    var keyboardType: UIKeyboardType = .default
    var submitLabel: SubmitLabel = .done
    var autocorrectDisabled: Bool = true
    var autocapitalization: InputFieldCapitalization = .sentences
    var textCase: Text.Case? = nil
    var characterLimit: Int = 1000
    /// View options
    var cornerRadius: CGFloat = 4
    var borderWidth: CGFloat = 1
    /// Focus state
    @FocusState private var isFocused: Bool
    var focusChanged: ((Bool) -> Void)?
    var onErrorState: ((Int) -> Void)?
    /// Private properties
    @State private var showPassword: Bool = false
    @State private var wasFocused: Bool = false
    private let autoCapitalizationMapping: [InputFieldCapitalization: TextInputAutocapitalization] = [
        .never: .never,
        .words: .words,
        .characters: .characters,
        .sentences: .sentences
    ]
    
    // MARK: - Computed properties
    var state: InputFieldState {
        if isFocused {
            return .active
        }
        if !isEnabled {
            return .disabled
        }
        if (showError && !errorText.isEmpty) && (wasFocused || shouldValidate) {
            return .error
        }
        return text.isEmpty ? .default : .filled
    }
    
    // MARK: - Body
    var body: some View {
        ZStack {
            VStack(alignment: .leading, spacing: 4) {
                HStack(spacing: 2) {
                    if title != "" {
                        Text(title)
                            .font(.label)
                            .lineSpacing(8)
                            .foregroundStyle(state.titleColor)
                    }
                    if isMandatory {
                        Text("*")
                            .font(.label)
                            .foregroundStyle(Color.textError)
                    }
                }
                if isPassword {
                    /// Password
                    ZStack(alignment: .trailing) {
                        secureField
                        /// Right button
                        Button(action: {
                            showPassword.toggle()
                        },
                               label: {
                            Image(showPassword
                                  ? "icon_password_visible" : "icon_password_hidden")
                            .renderingMode(.template)
                            .padding(8)
                            .background(.white)
                            .foregroundColor(Color.textLight)
                        })
                        .padding(8)
                    }
                } else {
                    /// Textfield
                    ZStack(alignment: .trailing) {
                        TextField("", text: $text.max(characterLimit), prompt: hintText)
                            .multilineTextAlignment(textAlignment)
                            .autocorrectionDisabled(autocorrectDisabled)
                            .focused($isFocused)
                            .font(.bodyRegular)
                            .foregroundStyle(state.textColor)
                            .padding()
                            .background(
                                RoundedRectangle(cornerRadius: cornerRadius)
                                    .fill(state.backgroundColor))
                            .overlay(
                                RoundedRectangle(cornerRadius: cornerRadius)
                                    .strokeBorder(state.borderColor,
                                                  style: StrokeStyle(lineWidth: borderWidth)))
                            .disabled(!isEnabled)
                            .keyboardType(keyboardType)
                            .submitLabel(submitLabel)
                            .textInputAutocapitalization(autoCapitalizationMapping[autocapitalization])
                            .textCase(textCase)
                            .allowsHitTesting(!isPicker)
                            .toolbar {
                                if (keyboardType == .numberPad || keyboardType == .phonePad) && isFocused {
                                    ToolbarItemGroup(placement: .keyboard) {
                                        Spacer()
                                        Button("btn_done".localized()) {
                                            isFocused = false
                                        }
                                        .buttonStyle(EIDButton(size: .small))
                                    }
                                }
                            }
                            .onChange(of: isFocused) { isFocused in
                                handleFocusChange(isFocused: isFocused)
                            }
                            .onChange(of: text) { newText in
                                handleTextCapitilization(newText: newText)
                            }
                        /// Right button
                        if rightIcon != .none && isEnabled {
                            Button(action: rightIconAction,
                                   label: {
                                Image(rightIcon.iconName)
                            })
                            .padding(8)
                        }
                    }
                }
                /// Error text
                if (state == .error || (state == .active && showError)) && !errorText.isEmpty {
                    Text(errorText)
                        .font(.tiny)
                        .lineSpacing(4)
                        .foregroundStyle(Color.textError)
                }
            }
            /// Picker tap action
            if isPicker {
                Button(action: {
                    tapAction()
                    if wasFocused == false {
                        wasFocused = true
                    }
                }, label: {
                    Color.clear
                })
            }
        }
    }
    
    // MARK: - Child views
    private var secureField: some View {
        let field = showPassword
        ? AnyView(TextField(hint, text: $text.max(characterLimit)))
        : AnyView(SecureField(hint, text: $text.max(characterLimit)))
        
        return field
            .multilineTextAlignment(textAlignment)
            .autocorrectionDisabled(autocorrectDisabled)
            .autocapitalization(.none)
            .focused($isFocused)
            .font(.bodyRegular)
            .foregroundStyle(state.textColor)
            .padding()
            .background(
                RoundedRectangle(cornerRadius: cornerRadius)
                    .fill(state.backgroundColor))
            .overlay(
                RoundedRectangle(cornerRadius: cornerRadius)
                    .strokeBorder(state.borderColor,
                                  style: StrokeStyle(lineWidth: borderWidth)))
            .disabled(!isEnabled)
            .keyboardType(keyboardType)
            .submitLabel(submitLabel)
            .onChange(of: isFocused) { isFocused in
                handleFocusChange(isFocused: isFocused)
            }
    }
    
    private var hintText: Text {
        Text(hint)
            .font(.bodyRegular)
            .foregroundColor(.textLight)
    }
    
    private func handleFocusChange(isFocused: Bool) {
        focusChanged?(isFocused)
        if wasFocused == false {
            wasFocused = true
        }
    }
    
    private func handleTextCapitilization(newText: String) {
        if case .characters = autocapitalization {
            text = newText.uppercased()
        } else {
            text = newText
        }
    }
}

// MARK: - Preview
struct FormInputField_Previews: PreviewProvider {
    static var previews: some View {
        VStack {
            EIDInputField(text: .constant("Test"),
                          showError: .constant(true),
                          errorText: .constant("True"))
        }
    }
}
