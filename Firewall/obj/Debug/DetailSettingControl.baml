   M S B A M L   `   `   `  ����C  ?Firewall, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:Firewall.ControllersFirewall.Controllers  5      #    Firewall.DetailSettingControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4local"clr-namespace:Firewall.Controllers     5      .��5	      �   J� IsManipulationEnabled  . .+�5
   
   �� -   5         ��Name$ saveBtn��5
      $
��Save��6"   $
��Left=�61   $��137,129,0,0q�6L   $	��Top=�6a   $��75��6y   6�   �� 5   
   -   5         ��Name$ bindAddrText��5      $
��Left=�6(   $��23��6C   $��	30,35,0,0q�6O    �� TextWrapping$
 Wrap=�6b   $�� ��6v   $	��Top=�6~   $	��103��6�   �� 5   
   -   5        $ 
fwPortText��5      $
��Left=�6&   $��23��6A   $��
193,35,0,0q�6M   $
 Wrap=�6a   $�� ��6u   $	��Top=�6}   $��50��6�   6�   �� 5   
   -   5         ��Name$ 	TypeCombo��5      $
��Left=�6&   $��
248,35,0,0q�6A   $	��Top=�6U   $��56��6m    �� SelectedIndex$ 0��6x   .@�5      �� $	��TCP��6   �� 5      $	��UDP��6   5      �� 5   
   -   5         ��Name$ bindAddrLbl��5      $��Bind Address��6%   $
��Left=�6<   $��	25,10,0,0q�6W   $	��Top=�6j   �� 5   
   -   5        $ 	fwPortLbl��5      $��FwPort��6#   $
��Left=�64   $��
193,10,0,0q�6O   $	��Top=�6c   �� 5   
   -   5        $ TypeLbl��5      $
��Type��6!   $
��Left=�60   $��
251,10,0,0q�6K   $	��Top=�6_   �� 5   
   -   5        $ nameText��5      $
��Left=�6$   $��23��6?   $��	30,88,0,0q�6K   $
 Wrap=�6^   $�� ��6r   $	��Top=�6z   $	��274��6�   �� 5   
   -	   5        $ nameLbl��5      $
��Name��6!   $
��Left=�60   $��	25,63,0,0q�6K   $	��Top=�6^   �� 5   
   -
   5        $ bindPortText��5      $
��Left=�6(   $��23��6C   $��
138,35,0,0q�6O   $
 Wrap=�6c   $�� ��6w   $	��Top=�6   $��50��6�   6�   �� 5   
   -   5        $ bindPortLbl��5      $
��Port��6%   $
��Left=�64   $��
137,10,0,0q�6O   $	��Top=�6c   �� 5   
   -   5        $ delBtn��5      $��Delete��6!   $
��Left=�62   $��229,129,0,0q�6M   $	��Top=�6b   $��75��6z   6�   �� 5   
   -   5        $ startBtn��5      $��Start��6#   $
��Left=�63   $��
44,129,0,0q�6N   $	��Top=�6b   $��75��6z   6�   �� 5   
   -   5        $ stopBtn��5      $
��Stop��6"   $
��Left=�61   $��
44,129,0,0q�6L   $	��Top=�6`   $��75��6x   $y�Hidden=�6�   6�   5      5      