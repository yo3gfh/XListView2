## XListView v1.0.1, a regular WinForms ListView, but with less flicker :-)
Copyright (c) 2025 Adrian Petrila, YO3GFH

-------
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
-------

**Features**

* exposes the DoubleBuffered property of the base class.
* exposes the OptimizeRedraw property which, when enabled, will block NM_CUSTOMDRAW forwarding during WM_NOTIFY processing, resulting in (hopefully) better performance, especially in SmallIcon or LargeIcon view mode.
