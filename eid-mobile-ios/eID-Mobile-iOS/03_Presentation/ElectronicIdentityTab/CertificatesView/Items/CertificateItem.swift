//
//  CertificateItem.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 15.04.24.
//

import SwiftUI


struct CertificateItem: View {
    // MARK: - Properties
    @State var serialNumber: String
    @State var validFrom: String
    @State var validUntil: String
    @State var deviceName: String
    @State var alias: String
    @State var status: CertificateStatus?
    @State var isExpiring: Bool
    @State var validCertificateOwner: Bool
    @Binding var hideItemMenuOptions: Bool
    @State var initCertificateAction: (CertificateAction) -> ()
    
    @State private var showOptions = false
    private let titleMinWidth: CGFloat = 128
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 16) {
            titleView
            VStack(spacing: 8) {
                aliasView
                validFromView
                validUntilView
                if isExpiring {
                    expiringView
                }
                deviceView
            }
            Divider()
            statusView
        }
        .onDisappear {
            showOptions = false
        }
        .padding()
        .frame(maxWidth: .infinity)
        .background(Color.backgroundWhite)
    }
    
    // MARK: - Child views
    private var titleView: some View {
        return HStack {
            Text(serialNumber)
                .font(.heading4)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
            
            Spacer()
            if status == .active || status == .stopped {
                menuOptionsButton
            }
        }
    }
    
    private var menuOptionsButton: some View {
        Button(action: {
            showOptions.toggle()
        }, label: {
            Image("icon_vertical_dots")
                .padding(4)
                .background(RoundedRectangle(cornerRadius: 8)
                    .fill(Color.backgroundLightGrey))
        })
        .popover(isPresented: $showOptions,
                 attachmentAnchor: .point(.center)) {
            menuOptions
                .presentationCompactAdaptation(.popover)
        }
    }
    
    private var aliasView: some View {
        HStack {
            Text("certificate_alias_title".localized())
                .font(.tiny)
                .foregroundStyle(Color.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(alias)
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var validFromView: some View {
        HStack {
            Text("certificate_valid_from_title".localized())
                .font(.tiny)
                .foregroundStyle(Color.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(validFrom.toDate()?.normalizeDate(outputFormat: .iso8601Date) ?? "")
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var validUntilView: some View {
        HStack {
            Text("certificate_valid_until_title".localized())
                .font(.tiny)
                .foregroundStyle(Color.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(validUntil.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601Date) ?? "")
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var expiringView: some View {
        return HStack {
            Spacer()
            Text("certificate_item_expires_soon_title".localized())
                .padding(2)
                .font(.extraTiny)
                .foregroundStyle(Color.textDefault)
                .textCase(.uppercase)
                .background(
                    RoundedRectangle(cornerRadius: 4,
                                     style: .continuous)
                    .fill(Color.buttonDangerHover)
                )
        }
    }
    
    private var deviceView: some View {
        HStack {
            Text("certificate_carrier_title".localized())
                .font(.tiny)
                .foregroundStyle(Color.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(deviceName)
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var statusView: some View {
        HStack {
            Text("certificate_status_title".localized())
                .font(.tiny)
                .foregroundStyle(Color.textDefault)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            HStack {
                Image(status?.iconName ?? "")
                Text(status?.title.localized() ?? "")
                    .font(.bodyRegular)
                    .foregroundStyle(status?.textColor ?? .textError)
            }
        }
    }
    
    private var menuOptions: some View {
        let statusAction = CertificateAction.action(status: status ?? .unknown)
        var actions = [CertificateAction]()
        switch status {
        case .active, .stopped:
            actions = [statusAction, CertificateAction.revoke]
            if status == .active && validCertificateOwner {
                actions.append(CertificateAction.changePIN)
            }
        default:
            actions = [statusAction]
        }
        return VStack(spacing: 0) {
            ForEach(actions, id: \.self) { action in
                Button(action: {
                    showOptions = false
                    initCertificateAction(action)
                }, label: {
                    MenuOptionItem(icon: action.icon,
                                   text: action.buttonTitle.localized(),
                                   textColor: action.textColor,
                                   iconColor: action.textColor,
                                   padding: 16,
                                   alignment: .leading)
                })
                
                if action != actions.last {
                    GradientDivider()
                }
            }
        }
    }
}

// MARK: - Preview
#Preview {
    CertificateItem(serialNumber: "1231231231231231231",
                    validFrom: "2024-05-14T13:50:43",
                    validUntil: "2024-05-14T13:50:43",
                    deviceName: "Name",
                    alias: "1231231231231231231",
                    status: .active,
                    isExpiring: true,
                    validCertificateOwner: false,
                    hideItemMenuOptions: .constant(true),
                    initCertificateAction: { _ in })
}
