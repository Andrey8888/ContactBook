   M S B A M L   `   `   `  ����F  BContactBook, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:ContactsBookContactsBook  5   	       ContactsBook.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:ContactsBook     5   	     � Title$  
MainWindow��5   	   $	��600��6   $
��1000��6)    � 	Resources 5	       �� 
TargetType �� Property �� Value)   #ContactsBook.SortIndicatorConverter   SortIndicatorConverter+   %ContactsBook.ColumnHighlightConverter  ColumnHighlightConverter��%x  '��       '�� H     '�� �     &	       &	 3    �� 5
   
   " ��6   .�5      ��  � ��$ 14(�6)   5      �� 5   
   " ��6   .�5      ��  � ��$ 14(�6)   5      �� 5   
   " ��6   .�5      ��  � ��$	 120��6&   �� 5       � ��$ 30��6'   �� 5       � ��$ 5q�6'   5        5   
   6'     5   
   6)   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5       $��* �6   �� 5!      $
��Auto �6   .+�5$   
   o� $��0��6   $��2��6   .@�5%      m� $��Файл��6   .@�5&      m� $��Открыть��6    m� Command 6,   �� *OpenCommand+m� 5'      $��Сохранить��6    6.   �� *SaveCommand+5(      5)      �� 5+   
   $��1��6   $��2��6"    �� Orientation$ 
Horizontal=�66   $��5q�6O   .+�5,      �� $��Фильтр: ��6   $��Center=�6(   �� 5-      $	��120��6   	 �� SelectedItem	 5.      �� *SelectedFilterProperty+��5/      �� *FilterProperties+�� 50      $	��200��6W   $��5,0,0,0q�6c   ��6   �� *
FilterText+
 �� UpdateSourceTrigger$
 PropertyChanged=�6c   51      �� 53   
   -   5         ��Name$ 	usersList��53   0   $��2��6   $��0��6    $��5q�54   <   ��53   C   �� *FilteredContacts+	 54      �� *SelectedContact+ �� View 55      �� 56      .P�57      �� $��Фамилия��6%    �� HeaderTemplate 58      �� 59      .R�5:   "   �� $��Surname��5;      $��0q�5@      $��Center=�5A      ��5:   )   �� *DataContext.SortCommand+ �� RelativeSource ��  �� AncestorType ListView��5<      �� *!DataContext.CurrentSortProperty+ ��  ListView �� 	Converter# [  �� ConverterParameter$ Surname��.��5B   &   �� $ 
Horizontal=�61   .+�5C   *   �� $��Фамилия��64   $
��Bold"�6C   �� 5D   *   ��64   �� *DataContext.SortDirection+ ��  ListView# [  $ Surname��5H   '   5I   #   5J       �� DisplayMemberBinding 5L      �� 5M       �� Path$ Surname�6&   5O      �� 5Q      $��Имя��6%    5R      �� 5S      .R�5T   "   �� $
��Name��5U      $��0q�5Z      $��Center=�5[      ��5T   )   �� *DataContext.SortCommand+ ��  ListView��5V      �� *!DataContext.CurrentSortProperty+ ��  ListView# [ $
 Name��.��5\   &   �� $ 
Horizontal=�61   .+�5]   *   �� $��Имя��64   $
��Bold"�6?   �� 5^   *   ��64   �� *DataContext.SortDirection+ ��  ListView# [  $
 Name��5b   '   5c   #   5d       5f      �� 5g      $
 Name�6&   5i      �� 5k      $��Отчество��6%    5l      �� 5m      .R�5n   "   �� $��
Patronymic��5o      $��0q�5t      $��Center=�5u      ��5n   )   �� *DataContext.SortCommand+ ��  ListView��5p      �� *!DataContext.CurrentSortProperty+ ��  ListView# [ $ 
Patronymic��.��5v   &   �� $ 
Horizontal=�61   .+�5w   *   �� $��Отчество��64   $
��Bold"�6D   �� 5x   *   ��64   �� *DataContext.SortDirection+ ��  ListView# [  $ 
Patronymic��5|   '   5}   #   5~       5�      �� 5�      $ 
Patronymic�6&   5�      �� 5�      $��Место работы��6%    5�      �� 5�      .R�5�   "   �� $��PlaceOfWork��5�      $��0q�5�      $��Center=�5�      ��5�   )   �� *DataContext.SortCommand+ ��  ListView��5�      �� *!DataContext.CurrentSortProperty+ ��  ListView# [ $ PlaceOfWork��.��5�   &   �� $ 
Horizontal=�61   .+�5�   *   �� $��Место работы��64   $
��Bold"�6H   �� 5�   *   ��64   �� *DataContext.SortDirection+ ��  ListView# [  $ PlaceOfWork��5�   '   5�   #   5�       5�      �� 5�      $ PlaceOfWork�6&   5�      �� 5�      $!��Номер телефона��6%    5�      �� 5�      .R�5�   "   �� $��PhoneNumber��5�      $��0q�5�      $��Center=�5�      ��5�   )   �� *DataContext.SortCommand+ ��  ListView��5�      �� *!DataContext.CurrentSortProperty+ ��  ListView# [ $ PhoneNumber��.��5�   &   �� $ 
Horizontal=�61   .+�5�   *   �� $!��Номер телефона��64   $
��Bold"�6J   �� 5�   *   ��64   �� *DataContext.SortDirection+ ��  ListView# [  $ PhoneNumber��5�   '   5�   #   5�       5�      �� 5�      $ PhoneNumber�6&   5�      5�      5�      �� 5�   
   $��3��6   $��0��6"   $ 
Horizontal=�62   $
��Left=�6K   $��5q�6f   .+�5�      �� $��Добавить��6   $��10q�6(   $��3q�64   ��6@   �� *
AddCommand+�� 5�      $��Изменить��6   $��10q�6(   ��64   �� *EditCommand+��5�      ��  �� ElementName$ 	usersList��$ SelectedItem��� 5�      $��Удалить��6   $��10q�6'   ��63   �� *DeleteCommand+��5�      �� $ 	usersList��$ SelectedItem�5�      5�      5�      