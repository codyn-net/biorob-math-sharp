dnl Warning: This is an automatically generated file, do not edit!
dnl Process this file with autoconf to produce a configure script.
AC_PREREQ([2.54])
AC_INIT([libbiorob-math1-sharp], [0.7])
AM_INIT_AUTOMAKE([foreign tar-ustar dist-xz no-dist-gzip])

AC_CONFIG_MACRO_DIR([m4])
AM_MAINTAINER_MODE

dnl pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
        AC_MSG_ERROR([You need to install pkg-config])
fi

LIBBIOROB_MATH_SHARP_API_VERSION=1.0
AC_SUBST(LIBBIOROB_MATH_SHARP_API_VERSION)

SHAMROCK_EXPAND_LIBDIR
SHAMROCK_EXPAND_BINDIR
SHAMROCK_EXPAND_DATADIR

AC_PROG_INSTALL

dnl check for dmcs first, then gmcs
AC_PATH_PROGS(CMCS, dmcs gmcs, no)

if test "x$CMCS" = "xno"; then
	AC_MSG_ERROR([dmcs nor gmcs found])
fi

AC_ARG_ENABLE(debug,
	AC_HELP_STRING([--enable-debug],
		[Use 'DEBUG' Configuration [default=YES]]),
		enable_debug=yes, enable_debug=no)
AM_CONDITIONAL(ENABLE_DEBUG, test x$enable_debug = xyes)
if test "x$enable_debug" = "xyes" ; then
	CONFIG_REQUESTED="yes"
fi
AC_ARG_ENABLE(release,
	AC_HELP_STRING([--enable-release],
		[Use 'RELEASE' Configuration [default=NO]]),
		enable_release=yes, enable_release=no)
AM_CONDITIONAL(ENABLE_RELEASE, test x$enable_release = xyes)
if test "x$enable_release" = "xyes" ; then
	CONFIG_REQUESTED="yes"
fi
if test -z "$CONFIG_REQUESTED" ; then
	AM_CONDITIONAL(ENABLE_DEBUG, true)
	enable_debug=yes
fi

dnl make sure we keep ACLOCAL_FLAGS around for maintainer builds to work
AC_SUBST(ACLOCAL_AMFLAGS, "$ACLOCAL_FLAGS -I m4")

dnl package checks, common for all configs

dnl package checks, per config


AC_CONFIG_FILES([
biorob-math-sharp.pc
Makefile
m4/Makefile
])

AC_OUTPUT
