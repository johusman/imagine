//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Standard_Machine_GUIs {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Standard_Machine_GUIs.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static System.Drawing.Bitmap Imagine_Adder {
            get {
                object obj = ResourceManager.GetObject("Imagine_Adder", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap Imagine_Branch {
            get {
                object obj = ResourceManager.GetObject("Imagine_Branch", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap Imagine_Ctrl_Contrast {
            get {
                object obj = ResourceManager.GetObject("Imagine_Ctrl_Contrast", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap Imagine_Ctrl_ControlMultiplier {
            get {
                object obj = ResourceManager.GetObject("Imagine_Ctrl_ControlMultiplier", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap Imagine_Gain {
            get {
                object obj = ResourceManager.GetObject("Imagine_Gain", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap Imagine_Img_Blur {
            get {
                object obj = ResourceManager.GetObject("Imagine_Img_Blur", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap Imagine_Img_Proximity {
            get {
                object obj = ResourceManager.GetObject("Imagine_Img_Proximity", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap Imagine_PythonScript {
            get {
                object obj = ResourceManager.GetObject("Imagine_PythonScript", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to import clr
        ///clr.AddReference(&apos;Imagine Library&apos;)
        ///from Imagine.Library import *
        ///from math import *
        ///
        ///max = ImagineColor.MAX
        ///
        ///def get(input, x, y):
        ///	col = input.GetPixel(x, y)
        ///	return [ col.A, col.R, col.G, col.B ]
        ///
        ///def set(input, x, y, a, r, g, b):
        ///	input.SetPixel(x, y, a, r, g, b)
        ///
        ///def fnorm(val):
        ///	return float(val) / max
        ///
        ///def isinside(input, x, y):
        ///	if (x &gt;= 0 and x &lt; input.Width and y &gt;= 0 and y &lt; input.Height):
        ///		return true
        ///	else:
        ///		return false
        ///
        ///def callback(val, total):
        ///	try:
        ///		_ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string pythonlib {
            get {
                return ResourceManager.GetString("pythonlib", resourceCulture);
            }
        }
    }
}