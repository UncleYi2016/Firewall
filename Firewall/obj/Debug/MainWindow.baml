   M S B A M L   `   `   `  ����C  ?Firewall, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:FirewallFirewall  5   	       Firewall.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Firewall     5   	   -   5          � Title$  
MainWindow��5   	   $	��600��6   $	��450��6)   .��5	      � -   5        
 � Tag$  ��5	   *    � RowDefinitions 5
   
   �� 5      $
��161* �6   �� 5      $
��293* �6   �� 5      $
��116* �6   .+�5   
   �� -   5         ��Name$ denyAddrText��5      $
��Left=�6(   $��	43,64,0,0q�6C    �� TextWrapping$
 Wrap=�6V   $�� ��6j   $	��Top=�6r   $��120                  ��6�   $  ��5      �� 5   
   -   5         ��Name$ 
denyAddBtn��5      $	��Add��6%   $
��Left=�63   $��
302,64,0,0q�6N   $	��Top=�6b   $��75��6z   6�   �� 5   
   -   5         ��Name$ label��5      $��Ban IP:��6   $
��Left=�61   $��	43,38,0,0q�6L   $	��Top=�6_   �� 5   
   -   5        $ 	statusLbl��5      $�� ��6#   $
��Left=�6.   $��43,0,0,0q�6I   $	��Top=�6[   $��75��6s   $	��334��6   $��2��6�   �� 5   
   -   5        $ bandwidthBtn��5      $	��Set��6'   $
��Left=�65   $��302,112,0,0q�6P   $	��Top=�6e   $��75��6}   6�   �� 5   
   -   5        $ bandwidthLbl��5      $��
Bandwidth:��6&   $
��Left=�6;   $��	43,88,0,0q�6V   $	��Top=�6i   �� 5   
   -	   5        $ bandwidthText��5      $
��Left=�6)   $��
43,112,0,0q�6D   $
 Wrap=�6X   $�� ��6l   $	��Top=�6t   $	��120��6�   �� 5   
   -
   5         ��Name$ bandwidthTypeCombo��5      $
��Left=�6/   $	��Top=�6J   $��70��6b   $��177,112,0,0q�6m    �� SelectedIndex$ 0��6�   .@�5      �� $	��B/s��6   �� 5      $
��KB/s��6   5      �� 5   
   -   5        	 ��Name$	 
tabControl��5      $
��Left=�6)   $	��231��6D   $��	43,20,0,0q�6Q   $��1��6d   $	��Top=�6q   $	��334��6�   5      �� 5   
   -   5        $ newConfigBtn��5      $	��New��6'   $
��Left=�65   $��302,261,0,0q�6P   $��1��6e   $	��Top=�6r   $��75��6�   6�   5      5      