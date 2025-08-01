//
//  PendingApprovalRequestItem.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 30.07.24.
//

import SwiftUI


struct PendingApprovalRequestItem: View {
    // MARK: - Properties
    @State var title: String = ""
    @State var details: String = ""
    @State var creationDate: String = ""
    @State var action: (ApprovalRequestStatus) -> Void
    private let verticalPadding: CGFloat = 36
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: verticalPadding) {
            titleView
            buttonsView
        }
        .padding()
        .frame(maxWidth: .infinity, minHeight: 80)
        .background(Color.backgroundWhite)
        .clipShape(RoundedRectangle(cornerRadius: 12))
        .addItemShadow()
        .padding([.leading, .trailing], 16)
        .padding([.top, .bottom], 8)
    }
    
    private var titleView: some View {
        return HStack(spacing: verticalPadding) {
            VStack(spacing: 8) {
                Text(title)
                    .font(.heading4)
                    .lineSpacing(8)
                    .foregroundStyle(Color.textActive)
                    .frame(maxWidth: .infinity, alignment: .leading)
                Text(details)
                    .font(.bodySmall)
                    .lineSpacing(8)
                    .foregroundStyle(Color.textDefault)
                    .frame(maxWidth: .infinity, alignment: .leading)
                Text(creationDate.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601Date) ?? "")
                    .font(.bodySmall)
                    .lineSpacing(8)
                    .foregroundStyle(Color.textDefault)
                    .padding(.top, 8)
                    .frame(maxWidth: .infinity, alignment: .leading)
            }
        }
    }
    
    private var buttonsView: some View {
            HStack {
                approveButton
                Spacer(minLength: 80)
                cancelButton
            }
    }
    
    private var approveButton: some View {
        Button(action: {
            action(.succeed)
        }, label: {
            Text("btn_accept")
        })
        .buttonStyle(EIDButton())
    }
    
    private var cancelButton: some View {
        Button(action: {
            action(.cancelled)
        }, label: {
            Text("btn_decline")
        })
        .buttonStyle(EIDButton(buttonState: .danger))
    }
}

#Preview {
    PendingApprovalRequestItem(title: "Auth",
                               details: "Request",
                               creationDate: "",
                               action: { _ in})
}
