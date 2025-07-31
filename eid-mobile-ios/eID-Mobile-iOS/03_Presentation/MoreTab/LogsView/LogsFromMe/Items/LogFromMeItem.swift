//
//  LogFromMeItem.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 25.01.24.
//

import SwiftUI


struct LogFromMeItem: View {
    // MARK: - Properties
    @State var eventId: String
    @State var eventDate: String
    @State var eventType: String
    private let titleMinWidth: CGFloat = 128
    
    // MARK: - Body
    var body: some View {
        VStack(alignment: .leading, spacing: 16) {
            Text(eventType)
                .font(.heading4)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
            VStack(spacing: 8) {
                HStack {
                    Text("identifier_type_title".localized())
                        .font(.tiny)
                        .foregroundStyle(Color.themePrimaryMedium)
                        .frame(minWidth: titleMinWidth, alignment: .leading)
                    Spacer()
                    Text(eventId)
                        .font(.bodyRegular)
                        .lineSpacing(8)
                        .foregroundStyle(Color.textDefault)
                        .multilineTextAlignment(.trailing)
                }
                Divider()
                HStack {
                    Text("logs_event_date_title".localized())
                        .font(.tiny)
                        .foregroundStyle(Color.themePrimaryMedium)
                        .frame(minWidth: titleMinWidth, alignment: .leading)
                    Spacer()
                    VStack(alignment: .trailing) {
                        Text(eventDate.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601Date) ?? "")
                            .font(.bodyRegular)
                            .lineSpacing(8)
                            .lineLimit(2)
                            .foregroundStyle(Color.textDefault)
                            .multilineTextAlignment(.trailing)
                        Text(eventDate.toDate()?.toLocalTime().normalizeDate(outputFormat: .fullTime) ?? "")
                            .font(.bodyRegular)
                            .lineSpacing(8)
                            .lineLimit(2)
                            .foregroundStyle(Color.textDefault)
                            .multilineTextAlignment(.trailing)
                    }
                }
                
            }
        }
        .padding()
        .frame(maxWidth: .infinity)
        .background(Color.backgroundWhite)
    }
}


// MARK: - Preview
#Preview {
    LogFromMeItem(eventId: "EventId",
                  eventDate: "Date",
                  eventType: "Type")
}
