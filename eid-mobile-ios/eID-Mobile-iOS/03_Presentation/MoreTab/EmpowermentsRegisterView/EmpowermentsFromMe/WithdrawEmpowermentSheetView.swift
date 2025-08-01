//
//  WithdrawEmpowermentSheetView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 13.05.25.
//

import SwiftUI


struct WithdrawEmpowermentSheetView: View {
    // MARK: - Properties
    var onButtonClick: (WithdrawEmpowermentSheetViewAction) -> ()
    
    // MARK: - Body
    var body: some View {
        VStack {
            Spacer()
            
            Text("withdraw_empowerment_alert_title".localized())
                .font(.heading4)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
                .padding(.bottom, 16)
                .frame(maxWidth: .infinity, alignment: .center)

            Text("withdraw_empowerment_alert_text".localized())
                .font(.bodyRegular)
                .lineSpacing(16)
                .foregroundStyle(Color.textDefault)
                .frame(maxWidth: .infinity, alignment: .leading)
            
            HStack {
                Button(action: {
                    onButtonClick(.confirm)
                }, label: {
                    Text("btn_confirm".localized())
                })
                .buttonStyle(EIDButton())
                
                Spacer(minLength: 80)
                
                Button(action: {
                    onButtonClick(.dismiss)
                }, label: {
                    Text("btn_decline".localized())
                })
                .buttonStyle(EIDButton(buttonState: .danger))
            }
            .padding(.top, 16)
            
            Spacer()
        }
        .padding()
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
    }
}

enum WithdrawEmpowermentSheetViewAction {
    case confirm, dismiss
    
}

// MARK: - Preview
#Preview {
    WithdrawEmpowermentSheetView(onButtonClick: {_ in })
}
