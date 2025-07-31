//
//  EmpowermentHistory.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 7.05.25.
//

import SwiftUI

struct EmpowermentHistory: View {
    // MARK: - Properties
    @State var empowerment: Empowerment
    
    // MARK: - Body
    var body: some View {
        VStack(alignment: .leading, spacing: 16) {
            Text("empowerment_history_title".localized())
                .font(.heading6)
                .lineSpacing(10)
                .foregroundStyle(Color.themeSecondaryDark)
            ForEach(empowerment.history, id: \.self) { event in
                switch event.status {
                case .collectingAuthorizerSignatures:
                    ForEach(empowerment.empowermentSignatures ?? [], id: \.self) { signature in
                        signerRow(for: event, with: signature)
                    }
                case .disagreementDeclared:
                    ForEach(empowerment.empowermentDisagreements, id: \.self) { disagreement in
                        disagreementRow(for: event, with: disagreement)
                    }
                default:
                    DetailsViewRow(title: event.status?.description.localized() ?? "",
                                   values: [event.dateTime?.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601DateTime) ?? ""])
                }
            }
        }
        .padding()
        .background(RoundedRectangle(cornerRadius: 8)
            .fill(Color.themePrimaryLight))
    }
    
    private func signerRow(for event: EmpowermentStatusHistory, with signature: EmpowermentSignatureResponse) -> some View {
        let date = event.dateTime?.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601DateTime) ?? ""
        let name = empowerment.authorizerUids.first(where: { $0.uid == signature.signerUid })?.name ?? ""
        return DetailsViewRow(title: event.status?.description.localized() ?? "",
                              values: [date + " " + name])
    }
    
    private func disagreementRow(for event: EmpowermentStatusHistory,
                                 with signature: EmpowermentDisagreement) -> some View {
        let date = event.dateTime?.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601DateTime) ?? ""
        let name = empowerment.empoweredUids.first(where: { $0.uid == signature.issuerUid })?.name ?? ""
        return DetailsViewRow(title: event.status?.description.localized() ?? "",
                              values: [date + " " + name])
    }
}
