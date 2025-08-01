//
//  EIDApplicationItem.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 7.03.24.
//

import SwiftUI


struct ApplicationItem: View {
    // MARK: - Properties
    @State var applicationNumber: String
    @State var applicationType: EIDApplicationType
    @State var creationDate: String
    @State var administratorName: String
    @State var deviceName: String
    @State var status: ApplicationStatus?
    @Binding var hideItemMenuOptions: Bool
    @State var initApplicationAction: (ApplicationAction) -> ()
    
    @State private var showOptions = false
    private let titleMinWidth: CGFloat = 128
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 16) {
            titleView
            VStack(spacing: 8) {
                createdOnView
                applicationTypeView
                administratorView
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
        HStack {
            Text(applicationNumber)
                .font(.heading4)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
            Spacer()
            if status == .pendingPayment {
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
    
    private var applicationTypeView: some View {
        HStack {
            Text("application_type_title".localized())
                .font(.tiny)
                .foregroundStyle(Color.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(applicationType.title.localized())
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var createdOnView: some View {
        HStack {
            Text("application_created_on_title".localized())
                .font(.tiny)
                .foregroundStyle(Color.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(creationDate.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601DateTime) ?? "")
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var administratorView: some View {
        HStack {
            Text("application_administrator_title".localized())
                .font(.tiny)
                .foregroundStyle(Color.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(administratorName)
                .font(.bodyRegular)
                .lineSpacing(8)
                .lineLimit(2)
                .foregroundStyle(Color.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var deviceView: some View {
        HStack {
            Text("application_carrier_title".localized())
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
        return HStack {
            Text("application_status_title".localized())
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
        let actions = [ApplicationAction.action(status: status ?? .unknown)]
        return VStack(spacing: 0) {
            ForEach(actions, id: \.self) { action in
                Button(action: {
                    showOptions = false
                    initApplicationAction(action)
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
    ApplicationItem(
        applicationNumber: "1234567890",
        applicationType: .issue,
        creationDate: "",
        administratorName: "Administrator",
        deviceName: "",
        status: .completed,
        hideItemMenuOptions: .constant(true),
        initApplicationAction: {_ in})
}
