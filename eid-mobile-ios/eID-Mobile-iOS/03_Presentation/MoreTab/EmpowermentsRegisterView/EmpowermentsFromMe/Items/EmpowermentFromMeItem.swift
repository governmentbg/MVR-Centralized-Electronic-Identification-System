//
//  EmpowermentFromMeItem.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 13.11.23.
//

import SwiftUI


struct EmpowermentFromMeItem: View {
    // MARK: - Properties
    @State var number: String
    @State var providerName: String
    @State var authorizerName: String
    @State var serviceName: String
    @State var authorizedId: String
    @State var authorizedIdsCount: Int
    @State var createdOn: String
    @State var shouldSign: Bool
    @State var status: EmpowermentStatus?
    @State var signAction: () -> Void
    @State var copyAction: () -> Void
    @State var withdrawAction: () -> Void
    @State private var showOptions = false
    private let titleMinWidth: CGFloat = 128
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 16) {
            HStack {
                EmpowermentItemNumber(empowermentNumber: number)
                Spacer()
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
            VStack(spacing: 8) {
                EmpowermentItemCreatedOn(createdOn: createdOn, minWidth: titleMinWidth)
                HStack {
                    Text("empowerments_from_me_on_behalf_of_title".localized())
                        .font(.tiny)
                        .foregroundStyle(Color.themePrimaryMedium)
                        .frame(minWidth: titleMinWidth, alignment: .leading)
                    Spacer()
                    Text(authorizerName)
                        .font(.bodyRegular)
                        .lineSpacing(8)
                        .foregroundStyle(Color.textDefault)
                        .multilineTextAlignment(.trailing)
                }
                EmpowermentItemProvider(providerName: providerName, minWidth: titleMinWidth)
                HStack {
                    Text("empowerment_service_title".localized())
                        .font(.tiny)
                        .foregroundStyle(Color.themePrimaryMedium)
                        .frame(minWidth: titleMinWidth, alignment: .leading)
                    Spacer()
                    Text(serviceName)
                        .font(.bodyRegular)
                        .lineSpacing(8)
                        .lineLimit(2)
                        .foregroundStyle(Color.textDefault)
                        .multilineTextAlignment(.trailing)
                }
                HStack {
                    Text("empowerments_from_me_authorized_id_title".localized())
                        .font(.tiny)
                        .foregroundStyle(Color.themePrimaryMedium)
                        .frame(minWidth: titleMinWidth, alignment: .leading)
                    Spacer()
                    VStack(alignment: .trailing) {
                        Text(authorizedId)
                            .font(.bodyRegular)
                            .lineSpacing(8)
                            .foregroundStyle(Color.textDefault)
                            .multilineTextAlignment(.trailing)
                        if authorizedIdsCount > 1 {
                            Text(String(format: "empowerments_from_me_authorized_ids_subtitle".localized(), authorizedIdsCount - 1))
                                .font(.tiny)
                                .foregroundStyle(Color.textActive)
                        }
                    }
                }
            }
            Divider()
            HStack {
                Text("empowerments_status_title".localized())
                    .font(.tiny)
                    .foregroundStyle(Color.textDefault)
                    .frame(minWidth: titleMinWidth, alignment: .leading)
                Spacer()
                viewForStatus()
            }
        }
        .padding()
        .frame(maxWidth: .infinity)
        .background(Color.backgroundWhite)
        .onDisappear {
            showOptions = false
        }
    }
    
    // MARK: - Child views
    private var menuOptions: some View {
        VStack(alignment: .leading, spacing: 0) {
            Button(action: {
                showOptions = false
                copyAction()
            }, label: {
                MenuOptionItem(icon: EmpowermentAction.copy.icon,
                               text: EmpowermentAction.copy.title.localized(),
                               textColor: EmpowermentAction.copy.textColor,
                               iconColor: EmpowermentAction.copy.textColor)
            })
            if status == .active {
                Divider()
                Button(action: {
                    showOptions = false
                    withdrawAction()
                }, label: {
                    MenuOptionItem(icon: EmpowermentAction.withdraw.icon,
                                   text: EmpowermentAction.withdraw.title.localized(),
                                   textColor: EmpowermentAction.withdraw.textColor,
                                   iconColor: EmpowermentAction.withdraw.textColor)
                })
            }
        }
    }
    
    private func viewForStatus() -> AnyView {
        switch status {
        case .created,
                .active,
                .denied,
                .disagreementDeclared,
                .withdrawn,
                .expired,
                .upcoming,
                .unknown,
                .unconfirmed,
                .none:
            return AnyView(statusItem(status: status))
        case .collectingAuthorizerSignatures, .awaitingSignature:
            return shouldSign ? AnyView(signButton) : AnyView(statusItem(status: status))
        case .collectingWithdrawalSignatures:
            return AnyView(Button(action: {
                print("Confirm withdrawal")
            }, label: {
                HStack {
                    Image(status?.iconName ?? "")
                        .tint(status?.textColor ?? .textError)
                    Text("empowerment_from_me_btn_confirm_withdrawal".localized())
                }
            })
                .buttonStyle(EIDButton(buttonType: .outline,
                                       buttonState: .danger,
                                       wide: false)))
        }
    }
    
    private var signButton: some View {
        Button(action: {
            signAction()
        }, label: {
            HStack {
                Image(status?.buttonIconName ?? "")
                    .renderingMode(.template)
                    .foregroundStyle(.white)
                    .font(.bodyRegular)
                Text("btn_sign".localized())
                    .font(.bodyRegular)
                    .foregroundStyle(.white)
            }
        }).buttonStyle(EIDButton(wide: false))
    }
    
    private func statusItem(status: EmpowermentStatus?) -> some View {
        HStack {
            Image(status?.iconName ?? "")
                .renderingMode(.template)
                .foregroundStyle(status?.textColor ?? .textError)
                .font(.bodyRegular)
            Text(status?.title.localized() ?? "")
                .font(.bodyRegular)
                .foregroundStyle(status?.textColor ?? .textError)
        }
    }
}


// MARK: - Preview
#Preview {
    EmpowermentFromMeItem(number: "sdk7686",
                          providerName: "Name",
                          authorizerName: "Authorizer name",
                          serviceName: "Service",
                          authorizedId: "AuthorizedId",
                          authorizedIdsCount: 1,
                          createdOn: "",
                          shouldSign: false,
                          status: .active,
                          signAction: {},
                          copyAction: {},
                          withdrawAction: {})
}
