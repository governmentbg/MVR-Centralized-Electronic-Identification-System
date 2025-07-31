//
//  EmpowermentToMeItem.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 20.11.23.
//

import SwiftUI


struct EmpowermentToMeItem: View {
    // MARK: - Properties
    @State var empowermentNumber: String
    @State var providerName: String
    @State var authorizerName: String
    @State var serviceName: String
    @State var createdOn: String
    @State var status: EmpowermentStatus?
    @Binding var hideItemMenuOptions: Bool
    @State var declareDisagreementAction: () -> Void
    
    @State private var showOptions = false
    private let titleMinWidth: CGFloat = 128
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 16) {
            HStack {
                EmpowermentItemNumber(empowermentNumber: empowermentNumber)
                Spacer()
                if status == .active {
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
            }
            VStack(spacing: 8) {
                EmpowermentItemCreatedOn(createdOn: createdOn, minWidth: titleMinWidth)
                HStack {
                    Text("empowerment_authorizer_title".localized())
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
            }
            Divider()
            HStack {
                Text("empowerments_status_title".localized())
                    .font(.tiny)
                    .foregroundStyle(Color.textDefault)
                    .frame(minWidth: titleMinWidth, alignment: .leading)
                Spacer()
                viewFor(status: status)
            }
        }
        .onChange(of: hideItemMenuOptions, perform: { newValue in
            if newValue {
                showOptions = false
            }
        })
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
                declareDisagreementAction()
            }, label: {
                MenuOptionItem(icon: EmpowermentAction.declareDisagreement.icon,
                               text: EmpowermentAction.declareDisagreement.title.localized(),
                               textColor: EmpowermentAction.declareDisagreement.textColor,
                               iconColor: EmpowermentAction.declareDisagreement.textColor)
            })
        }
    }
    
    private func viewFor(status: EmpowermentStatus?) -> some View {
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
    EmpowermentToMeItem(empowermentNumber: "Empowerment number",
                        providerName: "Name",
                        authorizerName: "Authorizer name",
                        serviceName: "Service",
                        createdOn: "",
                        status: .active,
                        hideItemMenuOptions: .constant(true),
                        declareDisagreementAction: {})
}
