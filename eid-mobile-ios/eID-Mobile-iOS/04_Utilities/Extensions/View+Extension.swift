//
//  View+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.10.23.
//

import SwiftUI


// MARK: - Border to selected edges
extension View {
    func border(width: CGFloat, edges: [Edge], color: Color) -> some View {
        overlay(EdgeBorder(width: width, edges: edges).foregroundColor(color))
    }
}


// MARK: - Shadow
extension View {
    func addItemShadow() -> some View {
        self.modifier(MenuItemShadowModifier())
    }
    
    func addGlow() -> some View {
        self.modifier(GlowModifier())
    }
}


// MARK: - Loading
extension View {
    func observeLoading(isLoading: Binding<Bool>, blurRadius: CGFloat = 2, details: String? = nil) -> some View {
        self.modifier(LoadingModifier(isLoading: isLoading,
                                      blurRadius: blurRadius,
                                      details: details))
    }
}

// MARK: - Present Error
extension View {
    func presentAlert(showAlert: Binding<Bool>, alertText: Binding<String>, onDismiss: (() -> Void)? = nil) -> some View {
        self.modifier(AlertPresenterModifier(showAlert: showAlert,
                                             alertText: alertText,
                                             onDismiss: onDismiss))
    }
    
    func presentMultipleButtonAlert(showAlert: Binding<Bool>, alertText: Binding<String>, buttons: [AlertButton]) -> some View {
        self.modifier(MultipleButtonAlertPresenterModifier(showAlert: showAlert,
                                                           alertText: alertText,
                                                           buttons: buttons))
    }
}

// MARK: - Present Action Sheet
extension View {
    func presentActionSheet(showOptions: Binding<Bool>, title: String, options: [String], onOptionPicked: ((String) -> Void)? = nil) -> some View {
        self.modifier(ActionSheetPresenterModifier(showOptions: showOptions,
                                                   title: title,
                                                   options: options,
                                                   onOptionPicked: onOptionPicked))
    }
}

// MARK: - Present Info Sheet
extension View {
    func presentInfoView(showInfo: Binding<Bool>, title: Binding<String>, description:  Binding<String>) -> some View {
        self.modifier(InfoViewModifier(showInfo: showInfo,
                                       title: title,
                                       description: description))
    }
}

// MARK: - Present Date Picker
extension View {
    func showDatePicker(showPicker: Binding<Bool>, selectedDate: Binding<Date>, rangeFrom: PartialRangeFrom<Date>? = nil, title: String, submitButtonTitle: String, onSubmit: (() -> Void)? = nil, canClear: Bool = false, clearButtonTitle: String = "", onClear: (() -> Void)? = nil) -> some View {
        self.modifier(DatePickerModifier(showPicker: showPicker,
                                         selectedDate: selectedDate,
                                         rangeFrom: rangeFrom,
                                         title: title,
                                         submitButtonTitle: submitButtonTitle,
                                         onSubmit: onSubmit,
                                         canClear: canClear,
                                         clearButtonTitle: clearButtonTitle,
                                         onClear: onClear))
    }
    
    func showDatePicker(showPicker: Binding<Bool>, selectedDate: Binding<Date>, rangeThrough: PartialRangeThrough<Date>? = nil, title: String, submitButtonTitle: String, onSubmit: (() -> Void)? = nil, canClear: Bool = false, clearButtonTitle: String = "", onClear: (() -> Void)? = nil) -> some View {
        self.modifier(DatePickerModifier(showPicker: showPicker,
                                         selectedDate: selectedDate,
                                         rangeThrough: rangeThrough,
                                         title: title,
                                         submitButtonTitle: submitButtonTitle,
                                         onSubmit: onSubmit,
                                         canClear: canClear,
                                         clearButtonTitle: clearButtonTitle,
                                         onClear: onClear))
    }
    
    func showDatePicker(showPicker: Binding<Bool>, selectedDate: Binding<Date>, range: ClosedRange<Date>? = nil, title: String, submitButtonTitle: String, onSubmit: (() -> Void)? = nil, canClear: Bool = false, clearButtonTitle: String = "", onClear: (() -> Void)? = nil) -> some View {
        self.modifier(DatePickerModifier(showPicker: showPicker,
                                         selectedDate: selectedDate,
                                         range: range,
                                         title: title,
                                         submitButtonTitle: submitButtonTitle,
                                         onSubmit: onSubmit,
                                         canClear: canClear,
                                         clearButtonTitle: clearButtonTitle,
                                         onClear: onClear))
    }
}


// MARK: - Add Navigation bar
extension View {
    func addNavigationBar<Content>(title: String, backgroundColor: UIColor = .themeSecondaryDark, tintColor: UIColor = .textWhite, backButtonHidden: Bool = true, @ToolbarContentBuilder content: () -> Content) -> some View where Content: ToolbarContent {
        self.modifier(SolidColorNavigationBarModifier(backgroundColor: backgroundColor,
                                                      tintColor: tintColor))
        .navigationTitle(title)
        .navigationBarTitleDisplayMode(.inline)
        .navigationBarBackButtonHidden(backButtonHidden)
        .toolbar(content: content)
    }
    
    func addTransparentGradientDividerNavigationBar<Content>(title: String? = nil, backgroundColor: UIColor = .themeSecondaryLight, tintColor: UIColor = .buttonDefault, backButtonHidden: Bool = true, @ToolbarContentBuilder content: () -> Content) -> some View where Content: ToolbarContent {
        self.modifier(GradientDividerNavigationBarModifier(backgroundColor: backgroundColor,
                                                           tintColor: tintColor))
        .navigationTitle(title ?? "")
        .navigationBarTitleDisplayMode(.inline)
        .navigationBarBackButtonHidden(backButtonHidden)
        .toolbar(content: content)
    }
}


// MARK: - Setup Tab Bar
extension View {
    func setupTabBar(itemWidth: CGFloat, height: CGFloat) -> some View {
        self.modifier(TabBarModifier(backgroundColor: .themeSecondaryDark,
                                     selectedColor: .themeSecondaryAlert,
                                     unselectedColor: .themeSecondaryLight))
        .toolbarBackground(.visible, for: .tabBar)
    }
}


// MARK: - Hide Tab Bar
extension View {
    func hidesTabBar() -> some View {
        self.toolbar(.hidden, for: .tabBar)
    }
}


// MARK: - Optional view modifier
extension View {
    /// Applies the given transform if the given condition evaluates to `true`.
    /// - Parameters:
    ///   - condition: The condition to evaluate.
    ///   - transform: The transform to apply to the source `View`.
    /// - Returns: Either the original `View` or the modified `View` if the condition is `true`.
    @ViewBuilder func `if`<Content: View>(_ condition: @autoclosure () -> Bool, transform: (Self) -> Content) -> some View {
        if condition() {
            transform(self)
        } else {
            self
        }
    }
}


// MARK: - Text field lenght limit
extension View {
    func limitLength(value: Binding<String>, length: Int) -> some View {
        self.modifier(TexfFieldLimitModifier(value: value,
                                             length: length))
    }
}


// MARK: - Hide Keyboard
#if canImport(UIKit)
extension View {
    func hideKeyboard() {
        UIApplication.shared.sendAction(#selector(UIResponder.resignFirstResponder), to: nil, from: nil, for: nil)
    }
}
#endif

extension View {
    @ViewBuilder func autoEdgePopover<Content: View>(
        isPresented: Binding<Bool>,
        attachmentAnchor: PopoverAttachmentAnchor = .rect(.bounds),
        @ViewBuilder content: @escaping () -> Content
    ) -> some View {
        if #available(iOS 18, *) {
            self
                .popover(isPresented: isPresented, attachmentAnchor: attachmentAnchor, content: content)
        } else {
            self
                .popover(isPresented: isPresented, attachmentAnchor: attachmentAnchor, content: content)
        }
    }
}
