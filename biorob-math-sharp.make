# Warning: This is an automatically generated file, do not edit!
ASSEMBLY_COMPILER_COMMAND = gmcs
COMPILE_TARGET = library
PROJECT_REFERENCES =

if ENABLE_DEBUG
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/Biorob.Math.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
BUILD_DIR = bin/Debug

BIOROB_MATH_DLL_MDB_SOURCE=bin/Debug/Biorob.Math.dll.mdb
BIOROB_MATH_DLL_MDB=$(BUILD_DIR)/Biorob.Math.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/Biorob.Math.dll
ASSEMBLY_MDB =
BUILD_DIR = bin/Release

BIOROB_MATH_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(BIOROB_MATH_DLL_MDB)

LINUX_PKGCONFIG = \
	$(BIOROB_MATH_SHARP_PC)


RESGEN=resgen2

all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG)

FILES = \
	AssemblyInfo.cs \
	Biorob.Math/Constants.cs \
	Biorob.Math/Expression.cs \
	Biorob.Math/IContextItem.cs \
	Biorob.Math/Instruction.cs \
	Biorob.Math/Operations.cs \
	Biorob.Math/Tokenizer.cs \
	Biorob.Math/Complex.cs \
	Biorob.Math.Solvers/Polynomial.cs \
	Biorob.Math.Solvers/Quadratic.cs \
	Biorob.Math.Solvers/Cubic.cs

DATA_FILES =

RESOURCES =

EXTRAS = \
	biorob-math-sharp.pc.in

REFERENCES =  \
	System

DLL_REFERENCES =

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG)

BIOROB_MATH_SHARP_PC = $(BUILD_DIR)/biorob-math-sharp-@LIBBIOROB_MATH_SHARP_API_VERSION@.pc
BIOROB_MATH_SHARP_API_PC = biorob-math-sharp-@LIBBIOROB_MATH_SHARP_API_VERSION@.pc

pc_files = $(BIOROB_MATH_SHARP_API_PC)

include $(top_srcdir)/Makefile.include

$(eval $(call emit-deploy-wrapper,BIOROB_MATH_SHARP_PC,$(BIOROB_MATH_SHARP_API_PC)))

$(BIOROB_MATH_SHARP_API_PC): biorob-math-sharp.pc
	cp $< $@

$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
