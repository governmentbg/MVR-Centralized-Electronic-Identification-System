//
//  EmpowermentsRegisterView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 31.10.23.
//

import SwiftUI


struct EmpowermentsRegisterView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    @State private var showInfo: Bool = false
    @Binding var path: [String]
    
    // MARK: - Body
    var body: some View {
        VStack {
            NavigationLink(value: MoreMenuDestinations.empowermentsFromMe.rawValue) {
                ListMenuItem(imageName: EmpowermentDirection.fromMe.iconName,
                             title: EmpowermentDirection.fromMe.title.localized(),
                             subtitle: EmpowermentDirection.fromMe.subtitle.localized())
            }
            .buttonStyle(ListMenuButtonStyle())
            
            NavigationLink(value: MoreMenuDestinations.empowermentsToMe.rawValue) {
                ListMenuItem(imageName: EmpowermentDirection.toMe.iconName,
                             title: EmpowermentDirection.toMe.title.localized(),
                             subtitle: EmpowermentDirection.toMe.subtitle.localized())
            }
            .buttonStyle(ListMenuButtonStyle())
            
            Spacer()
            
            NavigationLink(value: MoreMenuDestinations.createEmpowerment.rawValue) {
                Text("btn_new_empowerment".localized())
            }
            .buttonStyle(EIDButton(buttonType: .filled,
                                   buttonState: .success))
        }
        .padding()
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: "empowerments_screen_title".localized(),
                          content: {
            ToolbarItem(placement: .topBarLeading, content: {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            })
            ToolbarItem(placement: .topBarTrailing) {
                Button(action: {
                    showInfo = true
                }, label: {
                    Image("icon_info")
                        .renderingMode(.template)
                        .foregroundColor(Color.white)
                })
            }
        })
        .presentInfoView(showInfo: $showInfo,
                         title: .constant("info_title".localized()),
                         description: .constant("empowerment_register_info_description".localized()))
    }
}

extension EmpowermentsRegisterView {
    enum NavigationAction {
        case fromMe
        case toMe
        case createEmpowerment
        case noAction
    }
}


// MARK: - Preview
#Preview {
    EmpowermentsRegisterView(path: .constant([""]))
}
