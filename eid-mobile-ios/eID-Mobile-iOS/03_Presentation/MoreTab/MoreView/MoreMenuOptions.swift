//
//  MoreMenuOptions.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 27.10.23.
//

import Foundation


// MARK: - Menu groups
enum MoreMenuGroup: CaseIterable {
    case profile
    case services
    //    case useful
    //    case settings
    case bottom
}

extension MoreMenuGroup {
    var title: String {
        switch self {
        case .profile:
            return "more_profile_title"
        case .services:
            return "more_services_title"
            //        case .useful:
            //            return "more_useful_title"
            //        case .settings:
            //            return "more_settings_title"
        case .bottom:
            return ""
        }
    }
    
    var imageName: String? {
        switch self {
        case .profile:
            return "icon_profile"
        case .services:
            return "icon_services"
            //        case .useful:
            //            return "icon_info"
            //        case .settings:
            //            return "icon_settings"
        case .bottom:
            return nil
        }
    }
    
    var menuOptions: [MoreMenuOption] {
        switch self {
        case .profile:
            return [.profile,
                    .security,
                    .notificationSettings,
                    .paymentsHistory]
        case .services:
#if DEBUG
            return [.empowermentsRegister, .logs]
#else
            return UserManager.getUser()?.acr == .high ? [.empowermentsRegister, .logs] : [.logs]
#endif
            
        case .bottom:
            return [.faq,
                    .contactUs,
                    .termsAndConditions,
                    .administrators,
                    .centers,
                    .providers,
                    .secureDeliverySystem,
                    .onlineHelpSystem]
        }
    }
}


// MARK: - Menu Options
enum MoreMenuOption: String, CaseIterable {
    /** Profile */
    case profile = "profile_menu"
    case security = "device_security_menu_option"
    case notificationSettings = "more_notification_settings_menu"
    case changeEmail = "change_email_menu"
    case changePassword = "change_password_menu"
    case changePhone = "change_phone_menu"
    case paymentsHistory = "payments_history_menu"
    /** Services */
    case empowermentsRegister = "more_empowerments_register_menu"
    case logs = "more_logs_menu"
    /** Settings */
    case option1 = "more_option_1_menu"
    /** Bottom */
    case faq = "more_faq_menu"
    case contactUs = "more_contact_us_menu"
    case termsAndConditions = "more_terms_and_conditions_menu"
    case administrators = "more_administrators_menu"
    case centers = "more_centers_menu"
    case providers = "more_providers_menu"
    case secureDeliverySystem = "more_secure_delivery_system_menu"
    case onlineHelpSystem = "more_online_help_system_menu"
    
    var destination: MoreMenuDestinations {
        switch self {
        case .profile:
                .profile
        case .security:
                .security
        case .notificationSettings:
                .notificationSettings
        case .changeEmail:
                .changeEmail
        case .changePassword:
                .changePassword
        case .changePhone:
                .changePhone
        case .paymentsHistory:
                .paymentsHistory
        case .empowermentsRegister:
                .empowermentsRegister
        case .logs:
                .logs
        case .option1:
                .logs
        case .faq:
                .faq
        case .contactUs:
                .contactUs
        case .termsAndConditions:
                .termsAndConditions
        case .administrators:
                .administrators
        case .centers:
                .centers
        case .providers:
                .providers
        case .secureDeliverySystem:
                .secureDeliverySystem
        case .onlineHelpSystem:
                .onlineHelpSystem
        }
    }
}

extension MoreMenuOption {
    func localizedTitle() -> String {
        return rawValue.localized()
    }
}

// MARK: - Menu Destinations
enum MoreMenuDestinations: String {
    /** Profile */
    case profile = "Profile"
    case security = "Security"
    case notificationSettings = "Notification settings"
    case changeEmail = "Change email"
    case changePassword = "Change Password"
    case changePhone = "Change Phone"
    case paymentsHistory = "Payments History"
    /** Empowerments */
    case empowermentsRegister = "Eempowerments Register"
    case empowermentsFromMe = "Empowerments from me"
    case empowermentsToMe = "Empowerments to me"
    case createEmpowerment = "Create empowerment"
    /** Logs */
    case logs = "Logs"
    /** Bottom */
    case faq = "FAQ"
    case contactUs = "Contacts"
    case termsAndConditions = "Terms and conditions"
    case administrators = "Administrators"
    case centers = "Centers"
    case providers = "Providers"
    case secureDeliverySystem = "Secure delivery system"
    case onlineHelpSystem = "Online help system"
}
