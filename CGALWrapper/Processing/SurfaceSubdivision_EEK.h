#pragma once

#include "../CGALWrapper.h"

extern "C"
{
	CGALWRAPPER_API void* SurfaceSubdivision_EEK_Create();

	CGALWRAPPER_API void SurfaceSubdivision_EEK_Release(void* ptr);

	// 

	CGALWRAPPER_API void SurfaceSubdivision_EEK_Subdive_CatmullClark_PH(void* meshPtr, int iterations);

	CGALWRAPPER_API void SurfaceSubdivision_EEK_Subdive_DooSabin_PH(void* meshPtr, int iterations);

	CGALWRAPPER_API void SurfaceSubdivision_EEK_Subdive_Loop_PH(void* meshPtr, int iterations);

	CGALWRAPPER_API void SurfaceSubdivision_EEK_Subdive_Sqrt3_PH(void* meshPtr, int iterations);

	//Surface Mesh 

	CGALWRAPPER_API void SurfaceSubdivision_EEK_Subdive_CatmullClark_SM(void* meshPtr, int iterations);

	CGALWRAPPER_API void SurfaceSubdivision_EEK_Subdive_DooSabin_SM(void* meshPtr, int iterations);

	CGALWRAPPER_API void SurfaceSubdivision_EEK_Subdive_Loop_SM(void* meshPtr, int iterations);

	CGALWRAPPER_API void SurfaceSubdivision_EEK_Subdive_Sqrt3_SM(void* meshPtr, int iterations);

}