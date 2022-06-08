## Makefile
## Copyright 2003-2005  Nicolas Markey
#
# This work may be distributed and/or modified under the
# conditions of the LaTeX Project Public License, either version 1.3
# of this license or (at your option) any later version.
# The latest version of this license is in
#   http://www.latex-project.org/lppl.txt
# and version 1.3 or later is part of all distributions of LaTeX
# version 2003/12/01 or later.
#
# This work has the LPPL maintenance status "maintained".
# The Current Maintainer of this work is Nicolas Markey
#
# This work consists of the files
#       ttb_en.tex
#       ttb_en.sec1.tex  ttb_en.sec2.tex  ttb_en.sec3.tex
#       ttb_en.sec4.tex  ttb_en.sec5.tex  ttb_style.sty
#       local.bib        idxstyle.ist     Makefile
# and the derived ttb_en.dvi, ttb_en.ps and ttb_en.pdf

all:
	LATEX=pdflatex make tex

tex:	*.tex
	${LATEX} ttb_en.tex
	makeindex -s idxstyle.ist bibtex
	makeindex -s idxstyle.ist latex
	bibtex ttb_en
	${LATEX} ttb_en.tex
	${LATEX} ttb_en.tex

clean:
	rm -f *.aux ttb_en.log ttb_en.blg *.ilg *.ind *.idx \
	  ttb_en.bbl ttb_en.toc ttb_en.dvi ttb_en.out \
	  ttb_en.ptc* ttb_en.mtc* ttb_en.bmt *
~
