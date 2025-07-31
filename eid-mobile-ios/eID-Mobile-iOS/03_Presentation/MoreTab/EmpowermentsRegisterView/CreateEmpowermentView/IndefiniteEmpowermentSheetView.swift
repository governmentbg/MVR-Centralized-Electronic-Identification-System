//
//  IndefiniteEmpowermentSheetView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 22.01.25.
//

import SwiftUI


struct IndefiniteEmpowermentSheetView: View {
    // MARK: - Properties
    var onOKButtonClick: () -> ()
    
    // MARK: - Body
    private let alertText = ["indefinite_empowerment_alert_text_1".localized() + "\n",
                             "indefinite_empowerment_alert_text_2".localized() + "\n",
                             "indefinite_empowerment_alert_text_3".localized() + "\n",
                             "indefinite_empowerment_alert_text_4".localized() + "\n",
                             "indefinite_empowerment_alert_text_5".localized()]
    
    var body: some View {
        VStack {
            ForEach(0..<alertText.count, id: \.self) { index in
                switch index {
                case 1,2,3:
                    let segments = alertText[index].components(separatedBy: ":")
                    ForEach(0..<segments.count, id: \.self) { index in
                        Text(segments[index] + (index == 0 ? ":" : "" ))
                            .font(index == 0 ? .bodyBold : .bodyRegular)
                            .lineSpacing(8)
                            .foregroundStyle(Color.textDefault)
                            .frame(maxWidth: .infinity, alignment: .leading)
                    }
                default:
                    Text(alertText[index])
                        .font(.bodyRegular)
                        .lineSpacing(8)
                        .foregroundStyle(Color.textDefault)
                        .frame(maxWidth: .infinity, alignment: .leading)
                }
            }
            
            Button(action: {
                onOKButtonClick()
            }, label: {
                Text("btn_ok".localized())
            })
            .buttonStyle(EIDButton())
            .padding(.top, 16)
        }
        .padding()
    }
}

// MARK: - Preview
#Preview {
    IndefiniteEmpowermentSheetView(onOKButtonClick: {})
}
