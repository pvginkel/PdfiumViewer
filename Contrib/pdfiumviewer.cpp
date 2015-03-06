// Copyright (c) 2015 Pieter van Ginkel. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

#include "../../fpdfsdk/include/fpdfview.h"
#include "../../v8/include/v8.h"
#include "../../v8/include/libplatform/libplatform.h"

extern "C"
{
	DLLEXPORT void STDCALL FPDF_AddRef();
	DLLEXPORT void STDCALL FPDF_Release();
}

class RefCounter
{
private:
	CRITICAL_SECTION cs;
	int refCount;
	v8::Platform* platform;

public:
	RefCounter()
	{
		::InitializeCriticalSection(&cs);
		refCount = 0;
		platform = NULL;
	}

	~RefCounter()
	{
		::DeleteCriticalSection(&cs);
	}

	void Enter()
	{
		::EnterCriticalSection(&cs);
	}

	void Leave()
	{
		::LeaveCriticalSection(&cs);
	}

	void AddRef()
	{
		::EnterCriticalSection(&cs);

		if (refCount == 0)
		{
			v8::V8::InitializeICU();
			platform = v8::platform::CreateDefaultPlatform();
			v8::V8::InitializePlatform(platform);
			v8::V8::Initialize();

			FPDF_InitLibrary();
		}

		refCount++;

		::LeaveCriticalSection(&cs);
	}

	void Release()
	{
		::EnterCriticalSection(&cs);

		refCount--;

		if (refCount == 0)
		{
			FPDF_DestroyLibrary();
			v8::V8::ShutdownPlatform();
			delete platform;
		}

		::LeaveCriticalSection(&cs);
	}
};

static RefCounter refCounter;


DLLEXPORT void STDCALL FPDF_AddRef()
{
	refCounter.AddRef();
}

DLLEXPORT void STDCALL FPDF_Release()
{
	refCounter.Release();
}
